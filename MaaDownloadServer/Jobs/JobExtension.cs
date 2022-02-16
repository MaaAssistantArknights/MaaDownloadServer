using Quartz;

namespace MaaDownloadServer.Jobs;

public static class JobExtension
{
    public static void AddQuartzFetchGithubReleaseJob(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddQuartz(q =>
        {
            q.SchedulerId = "MaaServer-Download-Main-Scheduler";
            q.SchedulerName = "MaaDownloadServer Main Scheduler";
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(10);


            q.ScheduleJob<FetchGithubReleaseJob>(trigger =>
            {
                trigger.WithIdentity("Fetch-Github-Release-Trigger", "Download")
                    .WithCalendarIntervalSchedule(schedule =>
                    {
                        schedule.WithIntervalInMinutes(
                            Convert.ToInt32(configuration["MaaServer:GithubQuery:Interval"]));
                        schedule.InTimeZone(TimeZoneInfo.Local);
                        schedule.WithMisfireHandlingInstructionDoNothing();
                    })
                    .StartNow();
            }, job =>
            {
                job.WithIdentity("Fetch-Github-Release-Job", "Download");
            });

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
                    .StartAt(DateTimeOffset.Now.AddMinutes(1));
            }, job =>
            {
                job.WithIdentity("Public-Content-Check-Job", "Database");
            });

            q.ScheduleJob<GameDataUpdateJob>(trigger =>
            {
                trigger
                    .WithIdentity("GameData-Update-Trigger", "Resource")
                    .StartNow();
            }, job =>
            {
                job.WithIdentity("GameData-Update-Job", "Resource");
            });
        });
    }
}
