using Quartz;

namespace MaaDownloadServer.Jobs;

public static class JobExtension
{
    public static void AddQuartzJobs(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        List<ComponentConfiguration> componentConfigurations)
    {
        serviceCollection.AddQuartz(q =>
        {
            q.SchedulerId = "MaaServer-Download-Main-Scheduler";
            q.SchedulerName = "MaaDownloadServer Main Scheduler";
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(10);

            // 组件更新任务
            var componentCount = 1;
            foreach (var componentConfiguration in componentConfigurations)
            {
                var startDelay = 0 + componentCount * 0.5;
                q.ScheduleJob<PackageUpdateJob>(trigger =>
                {
                    trigger.WithIdentity($"Package-{componentConfiguration.Name}-Update-Trigger", "Package-Update-Trigger")
                        .WithCalendarIntervalSchedule(schedule =>
                        {
                            schedule.WithIntervalInMinutes(componentConfiguration.Interval);
                            schedule.InTimeZone(TimeZoneInfo.Local);
                            schedule.WithMisfireHandlingInstructionDoNothing();
                        })
                        .StartAt(DateTimeOffset.Now.AddMinutes(startDelay));
                }, job =>
                {
                    job.WithIdentity($"Package-{componentConfiguration.Name}-Update-Job", "Package-Update-Job");
                    IDictionary<string, object> data = new Dictionary<string, object> { { "configuration", componentConfiguration } };
                    job.SetJobData(new JobDataMap(data));
                });

                componentCount++;
            }

            // Public Content 过期检查任务
            q.ScheduleJob<PublicContentCheckJob>(trigger =>
            {
                trigger.WithIdentity("Public-Content-Check-Trigger", "Database")
                    .WithCalendarIntervalSchedule(schedule =>
                    {
                        schedule.WithIntervalInMinutes(
                            Convert.ToInt32(configuration["MaaServer:PublicContent:OutdatedCheckInterval"]));
                        schedule.InTimeZone(TimeZoneInfo.Local);
                        schedule.WithMisfireHandlingInstructionDoNothing();
                    })
                    .StartAt(DateTimeOffset.Now.AddMinutes(10));
            }, job =>
            {
                job.WithIdentity("Public-Content-Check-Job", "Database");
            });
        });
    }
}
