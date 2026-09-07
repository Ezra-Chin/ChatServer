using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class SharedFile
    {
        public string fileName { get; set; }
        public string sharedBy { get; set; }
        public byte[] data { get; set; }
        public string channelName { get; set; }

        public SharedFile()
        {
            
            sharedBy = "";
            data = null;
         
        }
    }
}
