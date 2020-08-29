using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ParthenonScheduler.Jobs
{
    class Base_Emails
    {
        #region Members
        // SMTP client
        private readonly SmtpClient _client = new SmtpClient("smtp.office365.com")
        {
            Port = 587,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("support@itspeex.com", "$upp0rt@cce$$")
        };

        // Server name
        protected readonly string _serverName = Dns.GetHostName();
        #endregion

        #region Email Methods
        public async Task<string> SendEmailWithCSV(string email, string subject, string body, string filename, string csv)
        {
            string result = "";

            try
            {
                Trace.TraceInformation($"Send email to {email}");
                using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(csv)))
                {
                    Attachment attachment = new Attachment(stream, new ContentType("text/csv"))
                    {
                        Name = filename
                    };

                    await SendEmail(email, subject, body, attachment);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception during sendemailwith csv {ex.Message}");
            }

            return result;
        }

        public async Task<string> SendEmail(string email, string subject, string body, Attachment attachment = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                Trace.TraceError($"Invalid input for email, subject, or body.");
                return string.Empty;
            }

            try
            {
                MailMessage mailMessage = GetMailWithImg(email, subject, body);

                if (attachment != null)
                    mailMessage.Attachments.Add(attachment);

                await _client.SendMailAsync(mailMessage);

                Trace.TraceInformation("Email sent!");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception during email send {ex.Message}");
            }

            return string.Empty;
        }

        private MailMessage GetMailWithImg(string email, string subject, string body)
        {
            MailMessage mail = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("support@itspeex.com"),
                Subject = subject,
            };
            mail.AlternateViews.Add(GetEmbeddedImage(body));
            mail.To.Add(email);
            return mail;
        }

        private AlternateView GetEmbeddedImage(string body)
        {
            LinkedResource athenaLogo = new LinkedResource(@"C:\AthenaUserPortal\Resources\athena_email.png");
            LinkedResource itspeexLogo = new LinkedResource(@"C:\AthenaUserPortal\Resources\itspeex_email.jpg");

            athenaLogo.ContentId = Guid.NewGuid().ToString();
            itspeexLogo.ContentId = Guid.NewGuid().ToString();

            string signature = "<p>Reply to this email if you require assistance.</p><p>Sent by " + _serverName + 
                          "</p><p style='margin: 0 0 7px 11px;'>" + 
                          "Thank you,</p><table><tr><td><p style='margin: 0 0 0 10px;'> iTSpeeX Support Team</p>" +
                          "<p style='margin: 0 0 0 10px;'>5412 Courseview Dr, Suite 200A, Mason, OH 45040</p>" +
                          "<p style='margin: 0 0 0 10px;'>+1 (833) 578-6470</p>" +
                          "<p style='margin: -2px 0 0 10px;'><a href='mailto: support@itspeex.com'>support@itspeex.com</a> | " +
                          "<a href='http://athenaworkshere.com/'>athenaworkshere.com</a></p></td>" +
                          $"<td><img style='display: inline;vertical-align: middle' src='cid:{ athenaLogo.ContentId }'/>" +
                          $"<img style='display: inline;vertical-align: middle' src='cid:{ itspeexLogo.ContentId }'/>" +
                          "</td></tr></table>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body + signature, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(athenaLogo);
            alternateView.LinkedResources.Add(itspeexLogo);
            return alternateView;
        }
        #endregion
    }
}
