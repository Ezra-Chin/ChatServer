using Chat;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace DuplexClient.src
{

    public partial class ChannelListPage : Page
    {
        private ChatContract.IChatService foob;
        private string userId;
        private CancellationTokenSource cancellationTokenSource;

        public ChannelListPage(string id, ChatContract.IChatService chatContract)
        {
            InitializeComponent();
            foob = chatContract;
            userId = id;
            WelcomeText.Text = $"Welcome, {id}!";
            LoadChannels();
        }
      
        public void ChannelListUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                LoadChannels();
            });
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

        //creates new channel
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