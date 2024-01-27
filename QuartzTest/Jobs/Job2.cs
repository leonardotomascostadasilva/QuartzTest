using Quartz;
using QuartzTest.Domain;

namespace QuartzTest.Jobs
{
    public class Job2 : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var myObj = context.JobDetail.JobDataMap["context"] as Context;

            Console.WriteLine($"{GetType().Name} Execution {myObj.Date} - {myObj.Name}");
        }
    }
}
