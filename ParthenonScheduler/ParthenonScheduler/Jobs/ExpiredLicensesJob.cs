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
    class ExpiredLicensesJob : Base_Emails, IJob
    {
        // Query param
        private readonly string subExps = "[sub_exps] = 1";

        public async Task Execute(IJobExecutionContext context)
        {
            LicensesDAL licensesDAL = new LicensesDAL();
            UsersDAL usersDAL = new UsersDAL();

            string subject = "ATHENA : Pending License Expirations";
            string body = $"<p>Attached is a spreadsheet with all Company Licenses expriring within the next { LicensesDAL._expLicensesDays } days.</p>" ;

            Trace.TraceInformation($"Expired Licenses job running");
            List<Users> subscribers = usersDAL.GetSubscribers(subExps);
            foreach (var user in subscribers)
            {
                try
                {
                    var licenses = licensesDAL.GetExpiringLicensesByCompanyId(user.CompanyId);

                    if (licenses.Count > 0)
                        await SendEmailWithCSV(user.Email, 
                                               subject, 
                                               body, 
                                               "ExpiringLicenses.csv",
                                               licenses.ToCsv());
                } catch(Exception e)
                {
                    Trace.TraceError($"Exception during sendemail for expired jobs {e.Message}");
                }
            }

            if (subscribers.Count == 0)
                Trace.TraceWarning("No license expiration subscribers");
        }
    }
}
