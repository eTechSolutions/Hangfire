using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Hangfire.Server
{
    public static class ServerStatusNotifier
    { 
        private static SmtpClient _smtpClient;

        private static readonly List<string> statusCodes = new List<string> { "Server timeout", "Multiple job failures" }; 
        
        public static void Notify(int statusIndex, string message)
        {
            var toEmail = ConfigurationManager.AppSettings["toEmail"];
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];
            var fromName = ConfigurationManager.AppSettings["fromName"];

            if (!string.IsNullOrEmpty(toEmail) && !string.IsNullOrEmpty(fromEmail) && !string.IsNullOrEmpty(fromName)) 
                throw new Exception("Email settings is incorrect or not set!"); 

            string[] receivers = toEmail.Split(',');

            if (receivers != null || receivers.Count() > 0) {
                
                string subject = statusCodes[statusIndex];

                _smtpClient = new SmtpClient();

                MailMessage mailMsg = new MailMessage();

                // Compose email
                foreach (string receiver in receivers)
                {
                    mailMsg.To.Add(receiver);
                }

                mailMsg.From = new MailAddress(fromEmail, fromName);

                // Subject and multipart/alternative Body
                mailMsg.Subject = subject;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));

                // Send the email
                _smtpClient.Send(mailMsg);
            }
        }
    }
}
