using ChatContract;
using DuplexClient.src;
using System;
using System.ServiceModel;
using System.Windows;

namespace DuplexClient
{
    public partial class MainWindow : Window
    {
        private IChatService foob;
        private ChatCallbackImpl foobCallback;
        IAsyncResult asyncResult;
        private DuplexChannelFactory<ChatContract.IChatService> factory;

        public MainWindow()
        {

            InitializeComponent();
            DuplexChannelFactory<IChatService> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:9000/Chat/Duplex";
            foobCallback = new ChatCallbackImpl();
            foobFactory = new DuplexChannelFactory<IChatService>(foobCallback, tcp, URL);

            foob = foobFactory.CreateChannel();

            //take who sent it and what they sent and run it immediately
            Closing += (s, e) =>
            {
                if (MainFrame.Content is src.ChannelView channelView)
                {
                    try
                    {
                        foob.LeaveChannel(channelView.userId);
                    }
                    catch 
                    {}
                    
                    try
                    {
                        foob.SignOut(channelView.userId);
                    }
                    catch 
                    {}
                }
                else if (MainFrame.Content is src.ChannelListPage channelListPage)
                {
                    try
                    {
                        foob.SignOut(channelListPage.userId);
                    }
                    catch 
                    {}
                }
            };
            MainFrame.Navigate(new src.LoginView(foob, foobCallback));
        }
    }
}