using Chat;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace DuplexClient.src
{
    public partial class PrivateChatView : Window
    {
        ChatContract.IChatService foob;
        private string sender;
        private ChatCallbackImpl callback;
        public string recipient { get; }
        private CancellationTokenSource cancellationTokenSource;

        public PrivateChatView(ChatContract.IChatService chatContract, string sender, string recipient, ChatCallbackImpl callback)
        {
            InitializeComponent();
            foob = chatContract;
            this.sender = sender;
            this.recipient = recipient;
            this.callback = callback;
            callback.privateChatView = this;
            RecipientText.Text = recipient;

            LoadChat();
        }

        public void PrivateChatViewUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                LoadChat();
            });
        }

        private async void LoadChat()
        {
            try
            {
                Task<List<Message>> task = new Task<List<Message>>(() => foob.GetPrivateMessages(sender, recipient));
                task.Start();
                List<Message> message = await task;
                PrivateMessageList.ItemsSource = message;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load chat", "Unable to Load Chat", MessageBoxButton.OK);
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = PrivateMessageTextBox.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            try
            {
                Task task = new Task(() => foob.SendPrivateMessage(this.sender, this.recipient, message));
                task.Start();
                await task;

                PrivateMessageTextBox.Clear();
                LoadChat();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Send Message", "Unable to send message", MessageBoxButton.OK);

            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
