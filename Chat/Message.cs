    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Chat
    {
        public class Message
        {
            public string sender { get; set; }   
            public string text { get; set; }
            public DateTime time { get; set; }
 


        public Message()
            {
                sender = "";
                text = "";
                time = DateTime.Now;
                
            }
        }
    }
