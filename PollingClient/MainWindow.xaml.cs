using System.ServiceModel;
using System.Windows;

namespace PollingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChannelFactory<ChatContract.IPollingChatService> factory;
        ChatContract.IPollingChatService foob;
        public MainWindow()
        {
            InitializeComponent();


            NetTcpBinding tcp = new NetTcpBinding();

            //REMINDER TO CHANGE THIS DURING PROD 
            string URL = "net.tcp://localhost:9000/Chat/Polling";

            factory = new ChannelFactory<ChatContract.IPollingChatService>(tcp, URL);

            foob = factory.CreateChannel();

            MainFrame.Navigate(new src.LoginView(foob));
        }
    }
}
