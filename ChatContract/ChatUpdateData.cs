using System.Runtime.Serialization;

namespace ChatContract
{
    [DataContract]
    public class ChatUpdateData
    {
        [DataMember]
        public string Type;

        [DataMember]
        public string Message;
    }
}