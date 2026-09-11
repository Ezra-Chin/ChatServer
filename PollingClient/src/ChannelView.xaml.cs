using Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PollingClient.src
{
    public partial class ChannelView : Page
    {
        private ChatContract.IPollingChatService foob;
        public string userId;
        private string channelName;
        private CancellationTokenSource cancellationTokenSource;
        private List<PrivateChatView> privateWindows = new List<PrivateChatView>();

        public ChannelView(string userId, string channelName, ChatContract.IPollingChatService foob)
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
                MessageBox.Show(
                    "An error occurred while loading channels.", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void StopPolling()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            foreach (PrivateChatView window in privateWindows.ToList())
            {
                window.Close();
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
                    MessageBox.Show(
                        "Channel not found", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);

                    NavigationService.GoBack();
                }

                MessageList.ItemsSource = channel.messages;
                MemberList.ItemsSource = channel.members;
                FileList.ItemsSource = channel.files;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to fetch channel",
                    "Error", 
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                    "Failed to send message", 
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foob.LeaveChannel(userId);
                StopPolling();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to leave channels", 
                    "Error", 
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Member_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (MemberList.SelectedItem == null)
            {
                return;
            }

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

        private async void FileList_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (!(FileList.SelectedItem is SharedFile file))
                return;

            string ext = Path.GetExtension(file.fileName);

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = file.fileName,
                DefaultExt = ext,
                AddExtension = true,
                Filter = $"{ext} file|*{ext}"
            };

            if (dialog.ShowDialog() != true)
                return;

            string savePath = dialog.FileName;
            if (!savePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                savePath += ext;

            try
            {
                byte[] bytes = await Task.Run(() => foob.DownloadFile(channelName, file.fileId));

                if (bytes == null || bytes.Length == 0)
                {
                    MessageBox.Show("No file found", "No file found");
                    return;
                }

                File.WriteAllBytes(savePath, bytes);
                MessageBox.Show($"Saved to:\n{savePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Download Error");
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
                    MessageBox.Show(
                        "File size exceeds 2MB", 
                        "Error", 
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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
                       "Error",
                       MessageBoxButton.OK)
                       MessageBoxImage.Error;
                    return;
                }
                await LoadChannel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                     "Failed to share file.",
                     "Error",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);
            }
        }
    }
}