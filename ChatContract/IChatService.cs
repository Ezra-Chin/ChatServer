using Chat;
using System.Collections.Generic;
using System.ServiceModel;

namespace ChatContract
{
    [ServiceContract(
        CallbackContract = typeof(IChatCallback))]
    public interface IChatService
    {

        [OperationContract]
        bool SignIn(string userId);


        [OperationContract]
        void SignOut(string userId);


        [OperationContract]
        List<Channel> GetChannels();


        [OperationContract]
        bool CreateChannel(
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
            string userId, string channelName,
            string message);


        [OperationContract]
        List<Message> GetPrivateMessages(
            string senderId,
            string recipientId);

        [OperationContract]
        void SendPrivateMessage(
            string senderId,
            string recipientId,
            string message);
        [OperationContract]
        List<Notification> GetNotifications(string userId);
        [OperationContract]
        Channel GetChannel(string channelName, string userId);

        [OperationContract]
        SharedFile ShareFile(
            string userId,
            string fileName,
            byte[] data,
            string channelName
            );

        [OperationContract]
        void MarkNotificationAsRead(Notification notification);
    }
}