using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

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
        void SendPrivateMessage(string senderId, string recipientId, string message);

        [OperationContract]
        FileStruct ShareFile(string userId, string fileName, byte[] data);

       

    }
}
