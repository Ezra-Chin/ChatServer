using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ChatContract
{
    public interface IChatCallback
    {

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(
            string sender,
            string message);


        [OperationContract(IsOneWay = true)]
        void ReceiveUpdate(
            string update);

    }
}