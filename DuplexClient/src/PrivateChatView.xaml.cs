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
        public string recipient { get; }

        public PrivateChatView(ChatContract.IChatService chatContract, string sender, string recipient)
        {
            InitializeComponent();
            foob = chatContract;
            this.sender = sender;
            this.recipient = recipient;
            RecipientText.Text = recipient;

            //test rmv if fails
            LoadChat();

            //StartPolling();

        }

        private async Task LoadChat()
        {
            try
            {
                //test rmv if fails
                List<Message> messages = await Task.Run(() => foob.GetPrivateMessages(sender, recipient));

                //uncomment if fails
                //Task<List<Message>> task = new Task<List<Message>>(() => foob.GetPrivateMessages(sender, recipient));
                //task.Start();
                //List<Message> message = await task;
                PrivateMessageList.ItemsSource = messages;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load chat", "Unable to Load Chat", MessageBoxButton.OK);
            }
        }

        //rmv if fails
        public void AddMessage(Message message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                List<Message> messages = PrivateMessageList.ItemsSource as List<Message>;
                if (messages == null)
                {
                    messages = new List<Message>();
                }
                else
                {
                    messages = new List<Message>(messages);
                }
                messages.Add(message);

                messages.Sort((a, b) => a.time.CompareTo(b.time));
            
                PrivateMessageList.ItemsSource = messages;
            }));
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
                //rmv if fails
                PrivateMessageTextBox.Clear();
                await Task.Run(() => foob.SendPrivateMessage(
                    this.sender,
                    this.recipient,
                    message));

                //uncomment if fails
                //Task task = new Task(() => foob.SendPrivateMessage(this.sender, this.recipient, message));
                //task.Start();
                //await task;

                //PrivateMessageTextBox.Clear();
                //await LoadChat();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Send Message", "Unable to send message", MessageBoxButton.OK);

            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //cancellationTokenSource.Cancel();
            Close();

        }


    }
}
