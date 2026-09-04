Chat Project Structure
Data Models
Channel.cs
namespace Chat
{
    public class Channel
    {
        public string channelName;
        public List<string> members;
        public List<SharedFile> files;

        public Channel()
        {
            channelName = "";
            members = new List<string>();
            files = new List<SharedFile>();
        }
    }
}
Message.cs
namespace Chat
{
    public class Message
    {
        public string sender;
        public string text;
        public DateTime time;

        public Message()
        {
            sender = "";
            text = "";
            time = DateTime.Now;
        }
    }
}
SharedFile.cs
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
User.cs
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
WCF Server Interface
ServerInterface.cs
namespace ServerInterface
{
    public class ServerInterface
    {
        [OperationContract]
        void SignIn(string userId);

        [OperationContract]
        void SignOut(string userId);

        [OperationContract]
        List<Channel> GetChannels();

        [OperationContract]
        void CreateChannel(string userId, string channelName);

        [OperationContract]
        void JoinChannel(string userId, string channelId);

        [OperationContract]
        void LeaveChannel(string userId, string channelId);

        [OperationContract]
        void SendMessage(string userId, string message);

        [OperationContract]
        void SendPrivateMessage(
            string senderId,
            string recipientId,
            string message
        );

        [OperationContract]
        FileStruct ShareFile(
            string userId,
            string fileName,
            byte[] data
        );
    }
}