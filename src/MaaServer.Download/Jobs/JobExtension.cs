using Quartz;

namespace MaaServer.Download.Jobs;

public static class JobExtension
{
    public static void AddQuartzFetchGithubReleaseJob(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddQuartz(q =>
        {
            q.SchedulerId = "MaaServer-Download-Main-Scheduler";
            q.SchedulerName = "MaaServer.Download Main Scheduler";
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
        });
    }
}
