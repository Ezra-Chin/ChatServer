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
        private List<PrivateChatView> privateWindows = new List<PrivateChatView>();

        //test rmv if fails
        private ChatCallbackImpl callback;
        public ChannelView(string userId, string channelName, ChatContract.IChatService foob, ChatCallbackImpl callback)
        {
            InitializeComponent();
            this.userId = userId;
            this.channelName = channelName;
            this.foob = foob;
            this.callback = callback;
            ChannelNameText.Text = channelName;
            
            //tell callback obj that thisis now the active channel
            callback.SetChannelView(this);

            LoadChannel();
        }
        public void ChannelUpdate(Channel channel)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if(channel == null)
                {
                    return;
                }
                if(channel.channelName != channelName)
                {
                    return;
                }
                MessageList.ItemsSource = new List<Message>(channel.messages);
                MemberList.ItemsSource = new List<string>(channel.members);
                FileList.ItemsSource = new List<SharedFile>(channel.files);
            }));
        }
        public void PrivateMessageUpdate(Message message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (message == null)
                {
                    return;
                }
                foreach (PrivateChatView window in privateWindows)
                {
                    if (window.recipient == message.sender)
                    {
                        window.AddMessage(message);
                        window.Activate();
                        return;
                    }
                }
                OpenPrivateChat(message.sender);
                foreach (PrivateChatView window in privateWindows)
                {
                    if (window.recipient == message.sender)
                    {
                        window.AddMessage(message);
                        break;
                    }
                }
            }));
        }
        public void FileUpdate(SharedFile file)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (file == null)
                {
                    return;
                }
                LoadChannel();
            }));
        }

        //uncomment if fails
        //public ChannelView(string userId, string channelName, ChatContract.IChatService foob)
        //{
        //    InitializeComponent();
        //    this.userId = userId;
        //    this.channelName = channelName;
        //    this.foob = foob;
        //    ChannelNameText.Text = channelName;
        //    StartPolling();
        //}

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
                //test rmv if fails
                Channel channel = await Task.Run(() => foob.GetChannel(channelName, userId));
                //uncomment if fails
                //Task<Channel> task = new Task<Channel>(() => foob.GetChannel(channelName, userId));
                //task.Start();
                //Channel channel = await task;

                if (channel == null)
                {
                    MessageBox.Show("Channel not found", "Channel not found", MessageBoxButton.OK);
                    NavigationService.GoBack();

                    //test rmv if fails
                    return;
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

        //send msg to current channel
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                //test rmv if fails
                MessageTextBox.Clear();
                await Task.Run(() => foob.SendMessage(userId, channelName, message));
                //uncomment if fails
                //Task task = new Task(() => foob.SendMessage(userId, channelName, message));
                //task.Start();
                //MessageTextBox.Clear();

                //await task;

                //uncomment if fails
                //await LoadChannel();

                //test - rmv if fails
                //MessageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to send message", "Failed to Send Message", MessageBoxButton.OK);
            }
        }

        //leave current channel
        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foob.LeaveChannel(userId);

                //test rmv if fails
                callback.ClearChannelView();

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to leave channels", "Failed to leave channel", MessageBoxButton.OK);
            }
        }

        //select a member then open a private chat
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

        //open a new window for private chat
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
            {
                return;
            }

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


                //test rmv if fails
                SharedFile file = await Task.Run(() => foob.ShareFile(userId, fileName, data, channelName));

                //uncomment if fails
                //Task<SharedFile> task = new Task<SharedFile>(() => foob.ShareFile(userId, fileName, data, channelName));
                //task.Start();
                //SharedFile file = await task;

                if (file == null)
                {
                    MessageBox.Show(
                       "Failed to share file.",
                       "File Sharing Error",
                       MessageBoxButton.OK);
                    return;
                }
                //await LoadChannel();
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