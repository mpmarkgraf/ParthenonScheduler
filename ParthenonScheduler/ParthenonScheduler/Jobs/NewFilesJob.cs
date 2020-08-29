using ParthenonScheduler.DAL;
using ParthenonScheduler.Models;
using Quartz;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ParthenonScheduler.Jobs
{
    class NewFilesJob : Base_Emails, IJob
    {
        // Query param
        private readonly string subFiles = "[sub_files] = 1";

        public async Task Execute(IJobExecutionContext context)
        {
            FilesDAL filesDAL = new FilesDAL();
            UsersDAL usersDAL = new UsersDAL();

            Trace.TraceInformation("Running the New Files Job.");
            List<Users> subscribers = usersDAL.GetSubscribers(subFiles);
            foreach (var user in subscribers)
            {
                try
                {
                    var files = filesDAL.GetRecentFiles(context.PreviousFireTimeUtc);
                    string subject = "ATHENA : Recent File Uploads";
                    string body = $"<p>Attached is a spreadsheet with recent files [{files.Count}] uploaded since the last email.</p>";

                    if (files.Count > 0)
                        await SendEmailWithCSV(user.Email,
                                           subject,
                                           body,
                                           "NewFiles.csv",
                                           files.ToCsv());
                }catch (Exception e)
                {
                    Trace.TraceError($"Exception during new files job. {e.Message}");
                }
            }

            if (subscribers.Count == 0)
                Trace.TraceWarning("No new files subscribers");
        }
    }
}
