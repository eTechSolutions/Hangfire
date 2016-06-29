using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire.Server
{
    public class ServerStatusNotifier
    {

        private string _fromEmail;
        private string _fromName;
        // we should probably protect these (DAPI)
        private IList<string> _receivers;
        private string _username;
        private string _password;
        private SmtpClient _smtpClient;

        public readonly List<string> statusCodes = new List<string> { "Server timeout", "Multiple job failures" }; 

        public ServerStatusNotifier(ServerStatusNotifierOptions options)
        {
            _fromEmail = options.FromEmail;
            _fromName = options.FromName;
            _receivers = options.Receivers;
            _username = options.Username;
            _password = options.Password;

            _smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            _smtpClient.Credentials = new System.Net.NetworkCredential(_username, _password);
        }

        public void Notify(int statusIndex, string message)
        {

            string subject = statusCodes[statusIndex];

            MailMessage mailMsg = new MailMessage();
                
            // Compose email
            foreach (string receiver in _receivers)
            {
                mailMsg.To.Add(receiver);
            }
      
            mailMsg.From = new MailAddress(_fromEmail, _fromName);

            // Subject and multipart/alternative Body
            mailMsg.Subject = subject;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));

            // Send the email
            _smtpClient.Send(mailMsg);
        }
    }
}
