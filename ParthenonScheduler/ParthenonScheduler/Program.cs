using ParthenonScheduler.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ParthenonScheduler
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Trace.TraceWarning("Started the Parthenon scheduler.");
            RunProgram().GetAwaiter().GetResult();

            Trace.TraceWarning("Waiting for exit signal.");
            Console.ReadKey();
            Trace.TraceError("Scheduler is exiting");
        }

        private static async Task RunProgram()
        {
            try
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                await scheduler.Start();

                IJobDetail expiredLicenseJob = JobBuilder.Create<ExpiredLicensesJob>()
                    .WithIdentity("ExpiredLicensesJob", "Emails")
                    .Build();

                IJobDetail newFilesJob = JobBuilder.Create<NewFilesJob>()
                    .WithIdentity("NewFilesJob", "Emails")
                    .Build();

                ITrigger expiredLicenseTrigger = TriggerBuilder.Create()
                    .WithIdentity("ExpiredLicensesTrigger", "Emails")
                    .StartNow()
                    .WithCronSchedule(ConfigurationManager.AppSettings["ExpiredLicensesCron"])
                    .Build();

                ITrigger newFilesTrigger = TriggerBuilder.Create()
                    .WithIdentity("NewFilesTrigger", "Emails")
                    .StartNow()
                    .WithCronSchedule(ConfigurationManager.AppSettings["NewFilesCron"])
                    .Build();

                await scheduler.ScheduleJob(expiredLicenseJob, expiredLicenseTrigger);
                await scheduler.ScheduleJob(newFilesJob, newFilesTrigger);
            }
            catch (Exception se)
            {
                Trace.TraceError($"Configuration error or such in quartz startup {se.Message}");
            }
        }
    }
}
