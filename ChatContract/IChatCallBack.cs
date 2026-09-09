using System.Collections.Generic;
using System.ServiceModel;
using Chat;

namespace ChatContract
{
    public interface IChatCallback
    {
        //rmv pass in if fails
        [OperationContract(IsOneWay = true)]
        void ChannelListUpdate(List<Channel> channels);

        //test - rmv if fails
        [OperationContract(IsOneWay = true)]
        void ChannelUpdate(Channel channel);

        [OperationContract(IsOneWay = true)]
        void PrivateMessageUpdate(Message message);

        [OperationContract(IsOneWay = true)]
        void FileUpdate(SharedFile file);

        //[OperationContract(IsOneWay = true)]
        //void ReceiveMessage(
        //    string sender,
        //    string message);


        //[OperationContract(IsOneWay = true)]
        //void ReceiveUpdate(
        //    string update);

    }
}