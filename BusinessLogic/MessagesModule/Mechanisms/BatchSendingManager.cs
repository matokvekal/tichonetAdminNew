using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {
    public class BatchSendingManager : IDisposable {

        public static BatchSendingManager NewInstance (BatchSendingTaskSettings Settings, MessagesModuleLogic Logic = null) {
            return new BatchSendingManager(Settings, Logic);
        }

        public readonly MessagesModuleLogic Logic;

        public IEnumerable<T> MakeCountAcceptableByProvider<T>(IEnumerable<T> messages, ISendServiceProvider provider)
            where T : IMessage
        {
            if (messages == null || !messages.Any())
                return null;
            var restrictions = provider.RestrictionData;
            var log = provider.RestrictionDataLog;
            var Now = DateTime.Now;

            bool dayLimited = restrictions.MaxMessagesInDay > 0;
            int dayLimit = dayLimited ?
                restrictions.MaxMessagesInDay - log.GetMessagesSendedForPeriodCount(new DateTimePeriod(Now.AddDays(-1), Now))
                : 0;
            bool hourLimited = restrictions.MaxMessagesInHour > 0;
            int hourLimit = hourLimited ?
                restrictions.MaxMessagesInHour - log.GetMessagesSendedForPeriodCount(new DateTimePeriod(Now.AddHours(-1), Now))
                : 0;

            if (!dayLimited && !hourLimited)
                return messages;

            int allowed = 0;
            if (dayLimited)
                allowed = dayLimit;
            if (hourLimited)
                allowed = Math.Min(allowed, hourLimit);

            if (allowed <= 0)
                return null;

            return messages.Take(allowed);
        }

        public void StoreSendProviderWorkHistory (ISendServiceProvider provider, DateTime PeriodStart, DateTime PeriodEnd, int MessagesSended) {
            var log = provider.RestrictionDataLog;
            log.RegisterJob(new DateTimePeriod(PeriodStart, PeriodEnd), MessagesSended);
            log.ForgotJobsEndedEarlierThen(DateTime.Now.AddDays(-1));
            provider.RestrictionDataLog = log;
        }

        public BatchSendingManager(BatchSendingTaskSettings settings, MessagesModuleLogic Logic = null) {
            AutoDisposeLogic = Logic == null;
            this.Logic = Logic ?? new MessagesModuleLogic();
            Settings = settings;
        }

        public IQueryable<IEmailServiceProvider> GetActiveEmailProviders () {
            return Logic.GetFilteredQueryable<tblEmailSenderDataProvider>()
                .Where(x => x.IsActive);
        }

        public IQueryable<ISmsServiceProvider> GetActiveSmsProviders() {
            return Logic.GetFilteredQueryable<tblSmsSenderDataProvider>()
                .Where(x => x.IsActive);
        }

        public IQueryable<tblMessage> GetPendingEmails () {
            var q = Logic.GetFilteredQueryable<tblPendingMessagesQueue>()
                .OrderByDescending(x => x.Priority)
                .Select(x => x.tblMessage)
                .Where(x => !x.IsSms && !x.tblPendingMessagesQueue.Deleted);
            if (Settings.MailsLimit > -1)
                q = q.Take(Settings.MailsLimit);
            return q;
        }

        public IQueryable<tblMessage> GetPendingSms() {
            var q = Logic.GetFilteredQueryable<tblPendingMessagesQueue>()
                .OrderByDescending(x => x.Priority)
                .Select(x => x.tblMessage)
                .Where(x => x.IsSms && !x.tblPendingMessagesQueue.Deleted);
            if (Settings.SmsLimit > -1)
                q = q.Take(Settings.SmsLimit);
            return q;
        }

        BatchSendingTaskSettings Settings;

        #region IDisposable Implementation
        bool disposedValue = false; // To detect redundant calls
        readonly bool AutoDisposeLogic;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    //stack.Remove(this);
                }
                if (AutoDisposeLogic)
                    Logic.Dispose();
                //dispose unmanaged resources here
                disposedValue = true;
            }
        }
        public bool IsDisposed { get { return disposedValue; } }



        public void Dispose() {
            Dispose(true);
        }
        #endregion
    }
}
