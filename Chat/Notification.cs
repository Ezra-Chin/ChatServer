using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class Notification
    {
        public string recipient { set; get;  }
        public string sender { set; get; }
        public Message message { set; get; }
        public bool read { get; set; }

        public Notification()
        {

        }
        public Notification(string recipient, string sender, Message message, bool read)
        {
            this.recipient = recipient;
            this.sender = sender;
            this.message = message;
            this.read = read;
        }
    }
}
