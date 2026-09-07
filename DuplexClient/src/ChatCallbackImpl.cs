using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatContract;
using System.Windows.Controls;


namespace DuplexClient.src
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ChatCallbackImpl : IChatCallback
    {
        //private MainWindow mainWindow;

        //test
        private ChannelListPage channelListPage;
        public ChatCallbackImpl (ChannelListPage channelListPage)
        {
            this.channelListPage = channelListPage;
        }
        public void ChannelListUpdate()
        {
            channelListPage.ChannelListUpdate();
        }

        //public ChatCallbackImpl(ChannelListPage page)
        //{
        //    this.page = page;
        //}

        //public void ChannelListUpdate()
        //{
        //    mainWindow.ChannelListUpdate();
        //}


        //private ChannelListPage channelListPage;

        //public ChatCallbackImpl(ChannelListPage channelListPage)
        //{
        //    this.channelListPage = channelListPage;
        //}

        //public void ChannelListUpdate()
        //{
        //    channelListPage.ChannelListUpdate();
        //}

        //public void ReceiveMessage(string sender, string message)
        //{ }

        //public void ReceiveUpdate(string update)
        //{ }
    }
} 
