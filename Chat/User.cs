using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class User
    {
        public string userId { get; set; }
        public string currentChannel { get; set; }
        public DateTime joinedChannelAt { get; set; }
        public User()
        {
            userId = "";
            currentChannel = null;
        }
    }
}
