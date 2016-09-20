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

        class TaskHandler {
            object _lock = new object();
            bool allowExecution = true;

            /// <summary>
            /// Returns True if task execution allowed.
            /// After task is done u SHOULD call 'ReportTaskEnded()'!
            /// </summary>
            /// <returns></returns>
            public bool GetSafeThreadPermission () {
                lock (_lock) {
                    if (allowExecution) {
                        allowExecution = false;
                        return true;
                    }
                    return false;
                }
            }
            
            public void ReportTaskEnded() {
                lock (_lock) {
                    if (allowExecution)
                        throw new InvalidOperationException("Some task reported end of execution, when no start of execution was registered.");
                    allowExecution = true;
                }
            }
        }

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

        static object ImmediateBatchCreation_L = new object();

        public static BatchCreationResult RunImmediateBatchCreation (tblMessageSchedule Schedule, int Priority, ISqlLogic SqlLogic, MessagesModuleLogic Logic) {
            lock (ImmediateBatchCreation_L) {
                using (var manager = BatchCreationManager.NewInstance(SqlLogic, Logic)) {
                    var creator = new BatchCreator(manager);
                    var result = creator.CreateBatch(Schedule, Priority);
                    manager.SaveResultsToDB(new[] { result });
                    return result;
                }
            }
        }

        static BatchSendingTaskSettings DefaultsBatchSendingTaskSettings = new BatchSendingTaskSettings();

        static TaskHandler ScheduledBatchSending_H = new TaskHandler();

        public static void RunScheduledBatchSending(MessagesModuleLogic Logic, BatchSendingTaskSettings Settings = null) {
            if (!ScheduledBatchSending_H.GetSafeThreadPermission())
                return;
            try {
                Settings = Settings ?? DefaultsBatchSendingTaskSettings;
                using (var manager = BatchSendingManager.NewInstance(Settings, Logic)) {
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
                    var emailProviders = manager.GetActiveEmailProviders().ToList().GetEnumerator();
                    var Emailer = new EmailSender(manager);
                    SendCycle(manager, allPendingMails, emailProviders, Emailer);
                    //SMS
                    var smsProviders = manager.GetActiveSmsProviders().ToList().GetEnumerator();
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
                    Logic.SaveChanges();
                    //Logic.Dispose();
                }
            }
            finally {
                ScheduledBatchSending_H.ReportTaskEnded();
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
                    manager.Logic.DeleteLazy(pendingChunk.Cast<tblMessage>().Select(x => x.tblPendingMessagesQueue));
                    //UPDATE DB AFTER StoreSendProviderWorkHistory somehow:
                    manager.Logic.SaveChanges();
                    Counter += getted;
                }
                if (allPendingCount == Counter)
                    break;
            }
        }
    }
}
