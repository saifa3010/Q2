using Hangfire;

namespace Infrastructure.Outbox
{
    /// <summary>
    /// Registers all Hangfire recurring jobs. Call once from Program.cs
    /// after app.UseHangfireDashboard() / app.UseHangfireServer().
    /// </summary>
    public static class HangfireJobsSetup
    {
        /// <summary>
        /// Schedules the OutboxProcessorJob to run every 10 seconds.
        /// Hangfire's minimum CRON granularity is 1 minute, so we use
        /// AddOrUpdate with a 10-second interval via the Cron helper.
        /// </summary>
        public static void RegisterRecurringJobs(this IRecurringJobManager manager)
        {
            manager.AddOrUpdate<OutboxProcessorJob>(
                recurringJobId: "outbox-Job",
                methodCall: job => job.ProcessAsync(),
                cronExpression: "*/1 * * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });
        }
    }
}
