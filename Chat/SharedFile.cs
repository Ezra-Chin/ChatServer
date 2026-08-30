using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class SharedFile
    {
        public string fileName;
        public string sharedBy;
        public byte[] data;

        public SharedFile()
        {
            
            sharedBy = "";
            data = null;
        }
    }
}
