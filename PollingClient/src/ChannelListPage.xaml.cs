using Chat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace PollingClient.src
{

    public partial class ChannelListPage : Page
    {
        private ChatContract.IPollingChatService foob;
        public string userId;
        private CancellationTokenSource cancellationTokenSource;

        public ChannelListPage(string id, ChatContract.IPollingChatService chatContract)
        {
            InitializeComponent();
            foob = chatContract;
            userId = id;
            WelcomeText.Text = $"Welcome, {id}!";
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
                    await LoadChannels();

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

        public async Task LoadChannels()
        {
            try
            {
                Task<List<Channel>> task = new Task<List<Channel>>(() => foob.GetChannels());
                task.Start();
                List<Channel> channels = await task;

                ChannelList.ItemsSource = channels;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading channels.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Channel channel = (Channel)button.DataContext;

            try
            {
                foob.JoinChannel(userId, channel.channelName);
                NavigationService.Navigate(
                    new ChannelView(userId, channel.channelName, foob));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to join channel",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void CreateChannel_Click(object sender, RoutedEventArgs e)
        {
            string channelName = ChannelNameTextBox.Text.Trim();
            ChannelNameTextBox.Text = channelName;

            if (string.IsNullOrWhiteSpace(channelName))
            {
                MessageBox.Show(
                    "Please enter a channel name.",
                    "Invalid Channel Name",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            try
            {
                Task<Boolean> task = new Task<Boolean>(() => foob.CreateChannel(userId, channelName));
                task.Start();
                Boolean res = await task;
                if (!res)
                {
                    MessageBox.Show(
                   "Channel Already Exist",
                   "Channel Already Created",
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning);
                }
                ChannelNameTextBox.Clear();

                LoadChannels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                  ex.Message,
                  "Failed to create channel",
                  MessageBoxButton.OK,
                  MessageBoxImage.Warning);
                return;
            }
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foob.SignOut(userId);
                StopPolling(); 
                NavigationService.Navigate(new LoginView(foob));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                  $"Failed to sign out",
                  "Error",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);
            }
        }
    }
}