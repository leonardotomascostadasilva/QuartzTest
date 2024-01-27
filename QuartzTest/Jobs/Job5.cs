﻿using Quartz;
using QuartzTest.Domain;

namespace QuartzTest.Jobs
{
    public class Job5 : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {

            Console.WriteLine($"{GetType().Name} Execution {DateTime.Now}");
        }
    }
}
