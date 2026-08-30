using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class User
    {
        public string userId;
        public string currentChannel;
        public User()
        {
            userId = "";
            currentChannel = null;
        }
    }
}
