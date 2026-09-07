using Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;



namespace DuplexClient.src
{
    public partial class ChannelView : Page
    {
        private ChatContract.IChatService foob;
        private string userId;
        private string channelName;
        private CancellationTokenSource cancellationTokenSource;
        private List<PrivateChatView> privateWindows = new List<PrivateChatView>();

        public ChannelView(string userId, string channelName, ChatContract.IChatService foob)
        {
            InitializeComponent();
            this.userId = userId;
            this.channelName = channelName;
            this.foob = foob;
            ChannelNameText.Text = channelName;
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
                    await LoadChannel();
                    await LoadNotifications();

                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }
        private async Task LoadNotifications()
        {
            try
            {
                Task<List<Notification>> task = new Task<List<Notification>>(() => foob.GetNotifications(userId));
                task.Start();
                List<Notification> notifications = await task;

                foreach (Notification notification in notifications)
                {
                    bool open = false;
                    foreach (PrivateChatView window in privateWindows)
                    {
                        if (window.recipient == notification.sender)
                        {
                            open = true;
                            break;
                        }
                    }
                    if (open == false)
                    {
                        OpenPrivateChat(notification.sender);

                    }
                    Task taskb = new Task(() => foob.MarkNotificationAsRead(notification));
                    taskb.Start();
                    await taskb;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Load Notifications");
            }
        }
        private async Task LoadChannel()
        {
            try
            {
                Task<Channel> task = new Task<Channel>(() => foob.GetChannel(channelName, userId));
                task.Start();
                Channel channel = await task;

                if (channel == null)
                {
                    MessageBox.Show("Channel not found", "Channel not found", MessageBoxButton.OK);
                    NavigationService.GoBack();
                }
                MessageList.ItemsSource = channel.messages;
                MemberList.ItemsSource = channel.members;
                FileList.ItemsSource = channel.files;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to fetch channel", "Unable to fetch channel ", MessageBoxButton.OK);
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                Task task = new Task(() => foob.SendMessage(userId, channelName, message));
                task.Start();
                MessageTextBox.Clear();

                await task;
                await LoadChannel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to send message", "Failed to Send Message", MessageBoxButton.OK);
            }
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foob.LeaveChannel(userId);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to leave channels", "Failed to leave channel", MessageBoxButton.OK);
            }
        }

        private void Member_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (MemberList.SelectedItem == null)
                return;

            string selectedUser = MemberList.SelectedItem as string;

            if (selectedUser == userId)
            {
                return;
            }
            OpenPrivateChat(selectedUser);



            MemberList.SelectedItem = null;
        }
        private void OpenPrivateChat(string recipient)
        {
            foreach (PrivateChatView window in privateWindows)
            {
                if (window.recipient == recipient)
                {
                    window.Activate();
                    return;
                }
            }

            PrivateChatView newWindow = new PrivateChatView(foob, userId, recipient);


            privateWindows.Add(newWindow);

            newWindow.Closed += (s, e) =>
            {
                privateWindows.Remove(newWindow);
            };

            newWindow.Show();
        }
        private void FileList_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedItem == null)
            {
                return;
            }

            SharedFile file = FileList.SelectedItem as SharedFile;

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.FileName = file.fileName;

            if (dialog.ShowDialog() != true)
                return;


            try
            {
                File.WriteAllBytes(dialog.FileName, file.data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                   "Failed to Downlaod File .",
                   "Downlaod Error",
                   MessageBoxButton.OK);
            }
        }
        private async void ShareFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            try
            {


                byte[] data = File.ReadAllBytes(dialog.FileName);
                if (data.Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("File size exceeds 2MB", "File Size Exceeded", MessageBoxButton.OK);
                    return;
                }
                string fileName = Path.GetFileName(dialog.FileName);
                string extension = Path.GetExtension(fileName);

                string[] allowedExtension = { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".txt" };
                if (!allowedExtension.Contains(extension))
                {
                    MessageBox.Show("File type is not supported");
                    return;
                }


                Task<SharedFile> task = new Task<SharedFile>(() => foob.ShareFile(userId, fileName, data, channelName));
                task.Start();
                SharedFile file = await task;

                if (file == null)
                {
                    MessageBox.Show(
                       "Failed to share file.",
                       "File Sharing Error",
                       MessageBoxButton.OK);
                    return;
                }
                await LoadChannel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                     "Failed to share file.",
                     "File Sharing Error",
                     MessageBoxButton.OK);
            }
        }

    }
}
