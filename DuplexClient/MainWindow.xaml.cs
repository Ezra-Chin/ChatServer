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
                    {
                        MessageBox.Show(
                            "Failed to leave channel",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    
                    try
                    {
                        foob.SignOut(channelView.userId);
                    }
                    catch 
                    {
                        MessageBox.Show(
                            "Failed to sign out",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else if (MainFrame.Content is src.ChannelListPage channelListPage)
                {
                    try
                    {
                        foob.SignOut(channelListPage.userId);
                    }
                    catch 
                    {
                        MessageBox.Show(
                            "Failed to sign out",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            };
            MainFrame.Navigate(new src.LoginView(foob, foobCallback));
        }
    }
}