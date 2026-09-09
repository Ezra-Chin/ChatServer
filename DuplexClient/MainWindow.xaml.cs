using ChatContract;
using DuplexClient.src;
using System;
using System.ServiceModel;
using System.Windows;

namespace DuplexClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private IChatService foob;
        private ChatCallbackImpl foobCallback;
        IAsyncResult asyncResult;
        private DuplexChannelFactory<ChatContract.IChatService> factory;

        //private ChatCallbackImpl callback;

        //private ProcessLongTask longTask;


        public MainWindow()
        {

            InitializeComponent();
            DuplexChannelFactory<IChatService> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //REMINDER TO CHANGE THIS DURING PROD 
            string URL = "net.tcp://localhost:9000/Chat/Duplex";
            foobCallback = new ChatCallbackImpl();
            foobFactory = new DuplexChannelFactory<IChatService>(foobCallback, tcp, URL);

            foob = foobFactory.CreateChannel();

            MainFrame.Navigate(new src.LoginView(foob, foobCallback));
        }
    }
}
