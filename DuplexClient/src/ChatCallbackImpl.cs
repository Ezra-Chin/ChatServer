using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatContract;
using System.Windows.Controls;
using Chat;


namespace DuplexClient.src
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ChatCallbackImpl : IChatCallback
    {
        //private MainWindow mainWindow;

        //test
        private ChannelListPage channelListPage;
        private ChannelView channelView;
        public void SetChannelListPage(ChannelListPage page)
        {
            channelListPage = page;
        }
        public void SetChannelView(ChannelView page)
        {
            channelView = page;
        }
        public void ClearChannelView()
        {
            channelView = null;
        }
        public void ChannelListUpdate(System.Collections.Generic.List<Channel> channels)
        {
            channelListPage?.ChannelListUpdate(channels);
        }
        public void ChannelUpdate(Channel channel)
        {
            channelView?.ChannelUpdate(channel);
        }
        private void PrivateMessageUpdate(Message message)
        {
            channelView?.PrivateMessageUpdate(message);
        }
        private void FileUpdate(SharedFile file)
        {
            channelView?.FileUpdate(file);
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
