using Chat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace PollingClient.src
{
    public partial class PrivateChatView : Window
    {
        ChatContract.IPollingChatService foob;
        private string sender;
        public string recipient { get; }
        private CancellationTokenSource cancellationTokenSource;

        public PrivateChatView(ChatContract.IPollingChatService chatContract, string sender, string recipient)
        {
            InitializeComponent();
            foob = chatContract;
            this.sender = sender;
            this.recipient = recipient;
            RecipientText.Text = recipient;
            Unloaded += (s, e) => StopPolling();


            StartPolling();
        }

        //Source: https://stackoverflow.com/questions/23340894/polling-the-right-way
        public async void StartPolling()
        {
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await LoadChat();

                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }
        private void StopPolling()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

         
        }

        private async Task LoadChat()
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
                await LoadChat();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Send Message", "Unable to send message", MessageBoxButton.OK);

            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            Close();
        }
    }
}