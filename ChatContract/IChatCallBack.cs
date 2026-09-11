using System.ServiceModel;

namespace ChatContract
{
    public interface IChatCallback
    {
        //call then proceed, no storing
        [OperationContract(IsOneWay = true)]
        void ChannelListUpdate();

        [OperationContract(IsOneWay = true)]
        void ChannelViewUpdate();

        [OperationContract(IsOneWay = true)]
        void PrivateChatViewUpdate();
    }
}