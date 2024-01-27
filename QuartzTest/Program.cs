//using System.Collections.Specialized;
//using Quartz;
//using Quartz.Impl;
//using Quartz.Logging;
//using QuartzTest;
//using QuartzTest.Jobs;
//using QuartzTest.Triggers;
//using static Quartz.Logging.OperationName;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
//// Grab the Scheduler instance from the Factory
//StdSchedulerFactory factory = new StdSchedulerFactory();
//IScheduler scheduler = await factory.GetScheduler();

//// and start it off
//await scheduler.Start();

////define my jobs
////await scheduler.AddTriggerJob<Job1>();
////await scheduler.AddTriggerJob<Job2>();

//await TriggerExtensions.AddTriggerWithSqlJob<Job1>();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Quartz;
using QuartzTest.Domain;
using QuartzTest.Jobs;
using QuartzTest.Triggers;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((cxt, services) =>
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.MaxBatchSize = 50;
            q.UseInMemoryStore();
            //q.UsePersistentStore(e =>e.UseMySql("Server=localhost;Database=Quartz;"));
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    }).Build();

var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();

//define my jobs
await scheduler.AddTriggerJob<Job1>(new Context(){Name = "Meu Job 1"});
await scheduler.AddTriggerJob<Job2>(new Context() { Name = "Meu Job 2" });

// will block until the last running job completes
await builder.RunAsync();