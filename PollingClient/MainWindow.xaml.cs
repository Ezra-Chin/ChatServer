using System.ServiceModel;
using System.Windows;

namespace PollingClient
{
    public partial class MainWindow : Window
    {
        ChannelFactory<ChatContract.IPollingChatService> factory;
        ChatContract.IPollingChatService foob;
        
        public MainWindow()
        {
            InitializeComponent();
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:9000/Chat/Polling";

            factory = new ChannelFactory<ChatContract.IPollingChatService>(tcp, URL);

            foob = factory.CreateChannel();

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
            MainFrame.Navigate(new src.LoginView(foob));
        }
    }
}
