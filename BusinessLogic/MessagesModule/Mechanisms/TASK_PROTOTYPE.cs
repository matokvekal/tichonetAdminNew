using System;
using System.Collections.Generic;
using Business_Logic.SqlContext;
using System.Linq;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Business_Logic.MessagesModule.InnerLibs.Text2Graph;
using Business_Logic.MessagesModule.DataObjects;
using System.Transactions;

namespace Business_Logic.MessagesModule.Mechanisms {


    public class BatchSendingTaskSettings {
        public int MailsLimit = -1;
        public int SmsLimit = -1;
    }

    public static class TASK_PROTOTYPE {

        public static List<Message> GetDemoMessages (MessagesModuleLogic logic, IMessageTemplate tmpl, ISqlLogic sqlLogic, bool isSms, int MaxCount = 0) {
            using (var mng = BatchCreationManager.NewInstance(sqlLogic, logic)) {
                var cltr = new MessageDataCollector(mng);
                var msgData = cltr.Collect(tmpl);
                var producer = new MessageProducer(tmpl,null, new DefaultMarkUpSpecification { NewLineSymbol = "\n" });
                List<Message> result = new List<Message>();
                int counter = 0;
                foreach (var data in msgData) {
                    producer.ChangeWildCards(data.wildCards);
                    foreach (var textData in data.TextProductionData) {
                        result.Add(producer.Produce(textData, isSms ? MessageType.Sms : MessageType.Sms));
                        counter++;
                        if (counter == MaxCount)
                            return result;
                    }
                }
                return result;
            }
        }

        public static void RunScheduledBatchCreation(DateTime StartDate, int minutesPeriod, ISqlLogic sqlLogic) {
            if (minutesPeriod < 1)
                throw new ArgumentOutOfRangeException("minutesPeriod should be greater or equal to 1");

            using (var manager = BatchCreationManager.NewInstance(StartDate, minutesPeriod, sqlLogic)) {
                var shedules = manager.GetActualMessageSchedules();
                var creator = new BatchCreator(manager);
                var results = new List<BatchCreationResult>();
                foreach (var sched in shedules)
                    results.Add(creator.CreateBatch(sched,0));
                manager.SaveResultsToDB(results);
            }
        }

        public static BatchCreationResult RunImmediateBatchCreation (tblMessageSchedule Schedule, int Priority, ISqlLogic SqlLogic, MessagesModuleLogic Logic) {
            using (var manager = BatchCreationManager.NewInstance(SqlLogic, Logic)) {
                var creator = new BatchCreator(manager);
                var result = creator.CreateBatch(Schedule, Priority);
                manager.SaveResultsToDB(new[] { result });
                return result;
            }
        }

        static BatchSendingTaskSettings DefaultsBatchSendingTaskSettings = new BatchSendingTaskSettings();

        public static void RunScheduledBatchSending(MessagesModuleLogic Logic, BatchSendingTaskSettings Settings = null) {
            Settings = Settings ?? DefaultsBatchSendingTaskSettings;
            using (var transaction = new TransactionScope()) {
                bool success = true;
                //TODO TOTAL REFACTOR ASAP!!!!!!!!!!!!!!!!!!!!!!!
                //TRY-CATCH and complete transaction?
                using (var manager = BatchSendingManager.NewInstance(Settings,Logic)) {
                    var allPendingMails = manager.GetPendingEmails();
                    var allPendingSms = manager.GetPendingSms();

                    var batches = allPendingMails
                        .Select(x => x.tblMessageBatch)
                        .Distinct()
                        .Concat(allPendingSms
                            .Select(x => x.tblMessageBatch)
                            .Distinct())
                        .ToArray();

                    //EMAIL
                    var emailProviders = manager.GetActiveEmailProviders().GetEnumerator();
                    var Emailer = new EmailSender(manager);
                    SendCycle(manager, allPendingMails, emailProviders, Emailer);
                    //SMS
                    var smsProviders = manager.GetActiveSmsProviders().GetEnumerator();
                    var Smser = new SmsSender(manager);
                    SendCycle(manager, allPendingSms, smsProviders, Smser);

                    foreach (var batch in batches) {
                        //check if batch finished
                        if (!batch.tblMessages.Where(x => (x.tblPendingMessagesQueue != null) && (!x.tblPendingMessagesQueue.Deleted)).Any()) {
                            batch.FinishedOn = DateTime.Now;
                        }
                        //TODO THIS CHECK IS NOT RIGHT DECISION
                        //add errors to batch if there are some
                        if (batch.tblMessages.Where(x => x.ErrorLog != null).Any()) {
                            (batch as IErrorLoged).AddError("Some errors have arisen at sending stage at " + DateTime.Now.ToShortDateString());
                        }
                        manager.Logic.SaveLazy(batch);
                    }

                    try { manager.Logic.SaveChanges(); } catch { success = false; }
                }
                Logic.Dispose();
                if(success) transaction.Complete();
            }
        }

        static void SendCycle <TSenderProvider,TMessage>
            (BatchSendingManager manager, IQueryable<TMessage> allPendingMsgs, IEnumerator<TSenderProvider> providers, IMessageSender<TSenderProvider, TMessage> MessageSender)
                    where TSenderProvider : ISendServiceProvider
                    where TMessage : IMessage 
        {
            int allPendingCount = allPendingMsgs.Count();
            if (allPendingCount == 0)
                return;
            IEnumerable<TMessage> pendingChunk = null;
            int Counter = 0;
            while (providers.MoveNext()) {
                var provider = providers.Current;
                pendingChunk = manager.MakeCountAcceptableByProvider(allPendingMsgs, provider);
                int getted = pendingChunk == null ? 0 : pendingChunk.Count();
                if (getted > 0) {
                    DateTime start = DateTime.Now;
                    MessageSender.SendBatch(pendingChunk, provider);
                    manager.StoreSendProviderWorkHistory(provider, start, DateTime.Now.AddMilliseconds(1), getted);
                    //UPDATE DB AFTER StoreSendProviderWorkHistory somehow:
                    manager.Logic.DeleteLazy(pendingChunk.Cast<tblMessage>().Select(x => x.tblPendingMessagesQueue));

                    Counter += getted;
                }
                if (allPendingCount == Counter)
                    break;
            }
        }
    }
}
