using System;
using System.ServiceModel;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(
                typeof(ChatService),
                new Uri(
                "net.tcp://localhost:9000/Chat"));

            host.AddServiceEndpoint(
                typeof(ChatContract.IChatService),
                new NetTcpBinding(),
                "Duplex");

            host.AddServiceEndpoint(
                typeof(ChatContract.IPollingChatService),
                new NetTcpBinding(),
                "Polling");

            host.Open();
            Console.WriteLine("Chat Server Running");
            Console.ReadLine();
            host.Close();
        }
    }
}