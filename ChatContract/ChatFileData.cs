using System.Runtime.Serialization;

namespace ChatContract
{
    [DataContract]
    public class ChatFileData
    {
        [DataMember]
        public string FileName;

        [DataMember]
        public string SharedBy;

        [DataMember]
        public byte[] Data;
    }
}