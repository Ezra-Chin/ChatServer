using System.Collections.Generic;

namespace Chat
{
    //stores private convo between two users
    public class PrivateChat
    {
        public string userId1 { get; set; }
        public string userId2 { get; set; }

        public List<Message> messages { get; set; } = new List<Message>();
    }
}
