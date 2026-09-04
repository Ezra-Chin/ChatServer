using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;

namespace PollingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChannelFactory<ServerInterface.ServerInterface> ChannelFactory;
        ServerInterface.ServerInterface foob;
        public MainWindow()
        {
            InitializeComponent();


            NetTcpBinding tcp = new NetTcpBinding();

            //REMINDER TO CHANGE THIS DURING PROD 
            string URL = "net.tcp://localhost:8000/ChatServer";

            ChannelFactory = new ChannelFactory<ServerInterface.ServerInterface>(tcp, URL);
        }
    }
}
