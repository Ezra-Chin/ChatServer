using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class PrivateChat
    {
        public string userId1 { get; set; }
        public string userId2 { get; set; }


        public List<Message> messages { get; set; } = new List<Message>();
  
    }
}
