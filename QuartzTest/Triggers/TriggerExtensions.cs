using Quartz;
using QuartzTest.Domain;
using System.Collections.Specialized;

namespace QuartzTest.Triggers
{
    public static class TriggerExtensions
    {
        public static async Task AddTriggerJob<TJob>(this IScheduler scheduler, Context context) where TJob : IJob
        {
            // define the job and tie it to our Job1 class
            var job = JobBuilder.Create<TJob>()
                .WithIdentity(typeof(TJob).Name, $"group-{Guid.NewGuid()}")
                .Build();

            job.JobDataMap["context"] = context;

            // Trigger the job to run now, and then repeat every 10 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"trigger-{Guid.NewGuid()}", $"group-{Guid.NewGuid()}")
                .StartNow()
                //.WithCronSchedule("0 0/1 * * * ?")
                .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(1)
                    .RepeatForever())
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        public static async Task AddTriggerWithSqlJob<TJob>() where TJob : IJob
        {
            // you can have base properties
            NameValueCollection properties = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
            properties["quartz.jobStore.dataSource"] = "default";
            properties["quartz.dataSource.default.connectionString"] = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=quartznet;Data Source=.\\sqlInstance;";
            properties["quartz.dataSource.default.provider"] = "SqlServer";
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.jobStore.useProperties"] = "true";
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";

            // and override values via builder
            IScheduler scheduler = await SchedulerBuilder.Create(properties)
                // default max concurrency is 10
                .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
                // this is the default 
                // .WithMisfireThreshold(TimeSpan.FromSeconds(60))
                .UsePersistentStore(x =>
                {
                    // force job data map values to be considered as strings
                    // prevents nasty surprises if object is accidentally serialized and then 
                    // serialization format breaks, defaults to false
                    x.UseProperties = true;
                    x.UseClustering();
                    // there are other SQL providers supported too 
                    x.UseSqlServer("Server=localhost;Database=Quartz;");
                    // this requires Quartz.Serialization.Json NuGet package
                    x.UseJsonSerializer();
                })
                // job initialization plugin handles our xml reading, without it defaults are used
                // requires Quartz.Plugins NuGet package
                .UseXmlSchedulingConfiguration(x =>
                {
                    x.Files = new[] { "~/quartz_jobs.xml" };
                    // this is the default
                    x.FailOnFileNotFound = true;
                    // this is not the default
                    x.FailOnSchedulingError = true;
                })
                .BuildScheduler();

            await scheduler.Start();
            // define the job and tie it to our Job1 class
            IJobDetail job = JobBuilder.Create<TJob>()
                .WithIdentity(typeof(TJob).Name, $"group-{Guid.NewGuid()}")
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"trigger-{Guid.NewGuid()}", $"group-{Guid.NewGuid()}")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
