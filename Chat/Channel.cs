using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class Channel
    {
        public string channelName;
        public List<string> members;
        public List<SharedFile> files;

        public Channel()
        {
            channelName = "";
            members = new List<string>();
            files = new List<SharedFile>();
        }
    }
}


