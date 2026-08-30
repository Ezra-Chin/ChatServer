using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class Message
    {
        public string sender;   
        public string text;
        public DateTime time;

        public Message()
        {
            sender = "";
            text = "";
            time = DateTime.Now;
        }
    }
}
