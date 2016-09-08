using FluentScheduler;
using ticonet.Scheduler.Tasks;

namespace ticonet.Scheduler
{
    public class TaskRegistry : Registry
    {
        public TaskRegistry()
        {
            // Run every week on Sunday at 00:30
            Schedule<ITaskPopulateLinesPlan>().ToRunEvery(1).Weeks().On(System.DayOfWeek.Sunday).At(00, 30);
            // Run every 10 seconds
            Schedule<ITaskSending>().ToRunEvery(10).Seconds();
        }
    }
}