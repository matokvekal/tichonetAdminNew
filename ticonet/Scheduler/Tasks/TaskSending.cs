using Business_Logic.MessagesModule;
using Business_Logic.MessagesModule.Mechanisms;
using log4net;

namespace ticonet.Scheduler.Tasks {
    public interface ITaskSending : IAbstractTask { }

    public class TaskSending : AbstractTask, ITaskSending {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TaskSending));


        public override void Execute() {
            using (var logic = new MessagesModuleLogic(new MessageContext())) {
                TASK_PROTOTYPE.RunScheduledBatchSending(logic, new BatchSendingTaskSettings {SmsLimit = 20, MailsLimit = 20 });
            }
        }
    }
}