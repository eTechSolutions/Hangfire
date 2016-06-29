using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire.Server
{
    public class ServerStatusNotifierOptions
    {

        public string FromEmail { get; private set; }
        public string FromName { get; private set; }
        // we should probably protect these (DAPI)
        public IList<string> Receivers { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        
        public ServerStatusNotifierOptions(string username, string password, string fromEmail, string fromName, IList<String> receivers)
        {
            FromEmail = fromEmail;
            FromName = fromName;           
            Receivers = receivers;
            Username = username;
            Password = password;
        }
    }
}
