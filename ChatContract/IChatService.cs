using System.Collections.Generic;
using System.ServiceModel;
using Chat;

namespace ChatContract
{
    [ServiceContract(
        CallbackContract = typeof(IChatCallback))]
    public interface IChatService
    {

        [OperationContract]
        void SignIn(string userId);


        [OperationContract]
        void SignOut(string userId);


        [OperationContract]
        List<Channel> GetChannels();


        [OperationContract]
        void CreateChannel(
            string userId,
            string channelName);


        [OperationContract]
        void JoinChannel(
            string userId,
            string channelName);


        [OperationContract]
        void LeaveChannel(
            string userId);


        [OperationContract]
        void SendMessage(
            string userId,
            string message);


        [OperationContract]
        void SendPrivateMessage(
            string senderId,
            string recipientId,
            string message);


        [OperationContract]
        SharedFile ShareFile(
            string userId,
            string fileName,
            byte[] data);

    }
}