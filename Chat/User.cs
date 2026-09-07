using System;

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
