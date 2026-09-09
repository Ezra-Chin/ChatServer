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
    public delegate void CreateChannel();

    public partial class MainWindow : Window
    {
        //uncooment if fails
        //private IChatService foob;
        //private IChatCallback foobCallback;
        //private CreateChannel createChannel;
        //IAsyncResult asyncResult;
        //private DuplexChannelFactory<ChatContract.IChatService> factory;


        //test rmv if fails
        private IChatService foob;
        private ChatCallbackImpl callback;
        private DuplexChannelFactory<ChatContract.IChatService> foobFactory;


        //private ChatCallbackImpl callback;

        //private ProcessLongTask longTask;


        public MainWindow()
        {
            InitializeComponent();
            //uncomment if fails
            //DuplexChannelFactory<IChatService> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //REMINDER TO CHANGE THIS DURING PROD 
            string URL = "net.tcp://localhost:9000/Chat/Duplex";
            //foobCallback = new ChatCallbackImpl(this);

            //uncomment if fails
            //foobFactory = new DuplexChannelFactory<IChatService>(foobCallback, tcp, URL);

            //rmv if fails
            callback = new ChatCallbackImpl();
            InstanceContext context = new InstanceContext(callback);
            foobFactory = new DuplexChannelFactory<IChatService>(context, tcp, new EndpointAddress(URL));



            foob = foobFactory.CreateChannel();

            MainFrame.Navigate(new src.LoginView(foob));
        }

        //rmv if fails
        public ChatCallbackImpl GetCallback()
        {
            return callback;
        }
    }
}
