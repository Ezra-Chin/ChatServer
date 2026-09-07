using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class Channel
    {
        public string channelName { get; set; }
        public List<string> members { get; set; }

        public List<SharedFile> files { get; set; }
        public List<Message> messages { get; set; }

        public Channel()
        {
            channelName = "";
            members = new List<string>();
            files = new List<SharedFile>();
            messages = new List<Message>();
        }
    }
}


