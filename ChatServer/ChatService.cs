using Chat;
using ChatContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ChatServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ChatService : IChatService, IPollingChatService
    {
        //test rmv if fails
        private static readonly Dictionary<string, IChatCallback> callbacks = new Dictionary<string, IChatCallback>();
        private static readonly object callbackLock = new object();


        //private static List<IChatCallback> clients = new List<IChatCallback>();
        //private static readonly object clientsLock = new object();

        //public ChatService()
        //{
        //    IChatCallback callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();
        //    lock (clientsLock)
        //    {
        //        clients.Add(callback);
        //    }
        //}

        //sign in to chat server
        public bool SignIn(string userId)
        {
            lock (Storage.LockObject)
            {
                //prevent duplicate user session
                if (Storage.Users.Any(x => x.userId == userId))
                {
                    return false;
                }

                Storage.Users.Add(
                new User
                {
                    userId = userId
                });
            }

            //test rmv if fails
            IChatCallback callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();
            lock(callbackLock)
            {
                callbacks[userId] = callback;
            }
            

            return true;
        }

        //sign user out & rmv user from channel
        public void SignOut(string userId)
        {
            lock (Storage.LockObject)
            {
                User user = Storage.Users.FirstOrDefault(x => x.userId == userId);

                if (user != null)
                {
                    LeaveChannel(userId);
                    Storage.Users.Remove(user);
                }
            }

            //test rmv if fails
            lock (callbackLock)
            {
                callbacks.Remove(userId);
            }
        }

        //returns list of available channels
        //uncomment if fails
        //public List<Channel> GetChannels()
        //{
        //    return Storage.Channels;
        //}

        //test rmv if fails
        public List<Channel> GetChannels()
        {
            lock (Storage.LockObject)
            {
                return Storage.Channels.Select(
                    x => new Channel
                    {
                        channelName = x.channelName,
                        members = new List<string>(x.members),
                        files = new List<SharedFile>(x.files),
                        messages = new List<Message>(x.messages)
                    })
                    .ToList();
            }
            
        }

        //create new channel
        public bool CreateChannel(string userId, string channelName)
        {
            lock (Storage.LockObject)
            {
                if (Storage.Channels.Any(
                    x => x.channelName == channelName))
                {
                    return false;
                }

                Storage.Channels.Add(
                    new Channel
                    {
                        channelName = channelName
                    });
            }

            //test uncomment if fails
            //OperationContext.Current.GetCallbackChannel<IChatCallback>().ChannelListUpdate();

            //test rmv if fails
            NotifyChannelList();

            //foreach (IChatCallback client in clients)
            //{
            //    try
            //    {
            //        client.ChannelListUpdate();
            //    }
            //    catch(Exception ex)
            //    {

            //    }
            //}
            return true;
        }

        //test rmv if fails
        //notify every client
        private void NotifyChannelList()
        {
            List<IChatCallback> clients;

            lock (callbackLock)
            {
                clients = callbacks.Values.ToList();
            }

            List<Channel> channels = GetChannels();

            foreach (IChatCallback callback in clients)
            {
                try
                {
                    callback.ChannelListUpdate(GetChannels());
                }
                catch (Exception e)
                {
                }
            }
        }

        //add user to a channel
        public void JoinChannel(
            string userId,
            string channelName)
        {
            //test rmv if fails
            List<string> membersToNotify;
            Channel channel;

            lock (Storage.LockObject)
            {
                channel = Storage.Channels.FirstOrDefault(x => x.channelName == channelName);
                if (channel == null)
                {
                    return;
                }
                //rmv user from previous channel
                foreach (Channel c in Storage.Channels)
                {
                    c.members.Remove(userId);
                }
                if (!channel.members.Contains(userId))
                {
                    channel.members.Add(userId);
                }

                User user =
                Storage.Users.First(
                    x => x.userId == userId);
                user.currentChannel = channelName;
                user.joinedChannelAt = DateTime.Now;
            }
            NotifyChannelMembers(channelName);
        }

            //uncoment if fails
            //lock (Storage.LockObject)
            //{
            //    Channel channel =
            //    Storage.Channels.FirstOrDefault(
            //        x => x.channelName == channelName);

            //    if (channel == null)
            //        return;

            //    //rmv user from previous channel
            //    foreach (Channel c in Storage.Channels)
            //    {
            //        c.members.Remove(userId);
            //    }

            //    //add user to selected channels
            //    channel.members.Add(userId);

            //    User user =
            //    Storage.Users.First(
            //        x => x.userId == userId);

            //    user.currentChannel =
            //        channelName;

            //    user.joinedChannelAt = DateTime.Now;
            //}
        }

        //rmv user from their curr channel
        public void LeaveChannel(
            string userId)
        {
            //test rmv if fails
            string oldChannel = null;
            lock (Storage.LockObject)
            {
                User user = Storage.Users.FirstOrDefault(
                    x => x.userId == userId);
                if (user != null)
                {
                    oldChannel = user.currentChannel;
                    foreach (Channel c in Storage.Channels)
                    {
                        c.members.Remove(userId);
                    }
                    user.currentChannel = null;
                }
            }
            if (oldChannel != null)
            {
                NotifyChannelMembers(oldChannel);
            }

            //uncomment if fails
            //foreach (Channel c in Storage.Channels)
            //{
            //    c.members.Remove(userId);
            //}

            //User user =
            //Storage.Users.FirstOrDefault(
            //    x => x.userId == userId);

            //if (user != null)
            //{
            //    user.currentChannel = null;
            //}
        }

        //Sends msg to public channel
        public void SendMessage(
            string userId,
            string channelName,
            string message)
        {
            //test rmv if fails
            Channel channel;
            lock(Storage.LockObject)
            {
                User user = Storage.Users.FirstOrDefault(x => x.userId == userId);
                if (user == null)
                {
                    return;
                }
                channel = Storage.Channels.FirstOrDefault(x => x.channelName == channelName);
                if (channel == null)
                {
                    return;
                }
                channel.messages.Add(new Message
                {
                    sender = userId,
                    text = message,
                    time = DateTime.Now
                });
            }

            NotifyChannelMembers(channelName);

            //uncomment if fails
            //User user =
            //Storage.Users.FirstOrDefault(
            //    x => x.userId == userId);

            //if (user == null)
            //{
            //    return;
            //}    

            //Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName.Equals(channelName));
            //if (channel == null)
            //{
            //    return;
            //}

            //channel.messages.Add(new Message
            //{
            //    sender = userId,
            //    text = message,
            //    time = DateTime.Now
            //});
        }

        //test rmv if fails
        private void NotifyChannelMembers(
            string channelName)
        {
            Channel channel;

            lock (Storage.LockObject)
            {
                channel =
                    Storage.Channels.FirstOrDefault(
                        x => x.channelName == channelName);

                if (channel == null)
                    return;

                // Make copy so we don't send live server object
                channel = new Channel
                {
                    channelName = channel.channelName,
                    members =
                        new List<string>(channel.members),
                    messages =
                        new List<Message>(channel.messages),
                    files =
                        new List<SharedFile>(channel.files)
                };
            }

            foreach (string member in channel.members)
            {
                IChatCallback callback = null;

                lock (callbackLock)
                {
                    callbacks.TryGetValue(
                        member,
                        out callback);
                }

                if (callback == null)
                    continue;

                try
                {
                    callback.ChannelUpdate(channel);
                }
                catch (Exception)
                {
                    // Dead callback ignored
                }
            }
        }

        //get private chat between 2 users
        public List<Message> GetPrivateMessages(string senderId, string recipientId)
        {
            //rmv if fails
            lock (Storage.LockObject)
            {
                PrivateChat privateChat =
                  Storage.PrivateChats.FirstOrDefault(x => (x.userId1 == senderId && x.userId2 == recipientId) || (x.userId1 == recipientId && x.userId2 == senderId));

                if (privateChat == null)
                {
                    return new List<Message>();
                }

                return privateChat.messages.OrderBy(x => x.time).ToList();
            }

            //uncomment if fails
            //PrivateChat privateChat =
            //      Storage.PrivateChats.FirstOrDefault(x => (x.userId1 == senderId && x.userId2 == recipientId) || (x.userId1 == recipientId && x.userId2 == senderId));
            
            //if (privateChat == null)
            //{
            //    return new List<Message>();
            //}

            //return privateChat.messages.OrderBy(x => x.time).ToList();
        }

        //send private msg to another user
        public void SendPrivateMessage(
            string senderId,
            string recipientId,
            string message)
        {
            //test rmv if fails
            Message newMessage;
            lock (Storage.LockObject)
            {
                User sender =
            Storage.Users.FirstOrDefault(
                x => x.userId == senderId);

                User receiver =
                Storage.Users.FirstOrDefault(
                    x => x.userId == recipientId);

                if (sender == null || receiver == null)
                {
                    return;
                }

                //only allow private msg in same channel
                if (sender.currentChannel !=
                   receiver.currentChannel)
                {
                    return;
                }

                PrivateChat privateChat =
                      Storage.PrivateChats.FirstOrDefault(x => (x.userId1 == senderId && x.userId2 == recipientId) || (x.userId1 == recipientId && x.userId2 == senderId));

                if (privateChat == null)
                {
                    privateChat = new PrivateChat
                    {
                        userId1 = senderId,
                        userId2 = recipientId
                    };

                    Storage.PrivateChats.Add(privateChat);
                }

                Message newMessage = new Message
                {
                    sender = senderId,
                    text = message,
                    time = DateTime.Now,
                };
                privateChat.messages.Add(newMessage);

                Storage.Notifications.Add(new Notification(recipientId, senderId, newMessage, false));

                // Push to recipient
                NotifyPrivateMessage(
                    recipientId,
                    newMessage);

                // Push to sender too
                NotifyPrivateMessage(
                    senderId,
                    newMessage);
            }

            //uncomment if fails
            //User sender =
            //Storage.Users.FirstOrDefault(
            //    x => x.userId == senderId);

            //User receiver =
            //Storage.Users.FirstOrDefault(
            //    x => x.userId == recipientId);

            //if (sender == null || receiver == null)
            //{
            //    return;
            //}

            ////only allow private msg in same channel
            //if (sender.currentChannel !=
            //   receiver.currentChannel)
            //{
            //    return;
            //}

            //PrivateChat privateChat =
            //      Storage.PrivateChats.FirstOrDefault(x => (x.userId1 == senderId && x.userId2 == recipientId) || (x.userId1 == recipientId && x.userId2 == senderId));

            //if (privateChat == null)
            //{
            //    privateChat = new PrivateChat
            //    {
            //        userId1 = senderId,
            //        userId2 = recipientId
            //    };

            //    Storage.PrivateChats.Add(privateChat);
            //}

            //Message newMessage = new Message
            //{
            //    sender = senderId,
            //    text = message,
            //    time = DateTime.Now,
            //};
            //privateChat.messages.Add(newMessage);

            //Storage.Notifications.Add(new Notification(recipientId, senderId, newMessage, false));
        }


        //test rmv if fails
        private void NotifyPrivateMessage(string userId, Message message)
        {
            IChatCallback callback = null;

            lock (callbackLock)
            {
                callbacks.TryGetValue(
                    userId,
                    out callback);
            }

            if (callback == null)
            {
                return;
            }

            try
            {
                callback.PrivateMessageUpdate(message);

            }
            catch (Exception)
            {
            }
        }

        public List<Notification> GetNotifications(string userId)
        {
            lock (Storage.LockObject)
            {
                return Storage.Notifications.Where(x => x.recipient == userId && !x.read).ToList();
            }
        }

        //test rmv if fails
        public void MarkNotificationAsRead(string recipient, string sender)
        {
            lock (Storage.LockObject)
            {
                foreach (Notification notification in Storage.Notifications)
                {
                    if (notification.recipient == recipient &&
                        notification.sender == sender)
                    {
                        notification.read = true;
                    }
                }
            }
        }

        //get channel info to user
        public Channel GetChannel(string channelName, string userId)
        {
            lock (Storage.LockObject)
            {
                Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName == channelName);

                if (channel == null)
                {
                    return null;
                }

                User user = Storage.Users.FirstOrDefault(x => x.userId == userId);

                if (user == null) 
                {
                    return null;
                }

                Channel result = new Channel
                {
                    channelName = channel.channelName,
                    members = new List<string>(channel.members),
                    files = new List<SharedFile>(channel.files),
                    //only show messages from when the user join
                    messages = channel.messages.Where(x => x.time >= user.joinedChannelAt).ToList()
                };

                return result;
            }
        }


        //uncomment if fails ig?
        //public void MarkNotificationAsRead(Notification n)
        //{
        //    string recipient = n.recipient;
        //    string sender = n.sender;
        //    lock (Storage.LockObject)
        //    {
        //        foreach (Notification notification in Storage.Notifications)
        //        {
        //            if (notification.recipient == recipient &&
        //                notification.sender == sender)
        //            {
        //                notification.read = true;
        //            }
        //        }
        //    }
        //}

        //public void MarkNotificationAsRead(Notification notification)
        //{
        //    lock (Storage.LockObject)
        //    {
        //        Notification existingNotification = Storage.Notifications.FirstOrDefault(x => x.recipient == notification.recipient && x.sender == notification.sender && x.message == notification.message && !x.read);

        //        if (existingNotification != null)
        //        {
        //            Console.WriteLine("Read");
        //            existingNotification.read = true;
        //        }
        //    }
        //}
        
        public SharedFile ShareFile(
            string userId,
            string fileName,
            byte[] data,
            string channelName)
        {
            SharedFile file;
            //test rmv if fails
            lock (Storage.LockObject)
            {
                if (data.Length > 2 * 1024 * 1024)
                {
                    return null;
                }

                Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName.Equals(channelName));

                if (channel == null)
                {
                    return null;
                }

                file =
                new SharedFile
                {
                    fileName = fileName,
                    sharedBy = userId,
                    data = data
                };

                channel.files.Add(file);               
            }

            NotifyFile(channelName, file);
            return file;

            //uncomment if fails
            //if (data.Length > 2 * 1024 * 1024)
            //    return null;

            //Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName.Equals(channelName));
            
            //if (channel == null)
            //{
            //    return null;
            //}

            //SharedFile file =
            //new SharedFile
            //{
            //    fileName = fileName,
            //    sharedBy = userId,
            //    data = data
            //};

            //channel.files.Add(file);

            //return file;
        }


        //test rmv if fails
        private void NotifyFile(string channelName, SharedFile file)
        {
            List<string> members;
            lock (Storage.LockObject)
            {
                Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName == channelName);
                if (channel == null)
                { 
                    return;
                }
                members = new List<string>(channel.members);
            }

            foreach (string member in members)
            {
                IChatCallback callback = null;
                lock (callbackLock)
                {
                    callbacks.TryGetValue(member, out callback);
                }
                if (callback == null)
                {
                    continue;
                }
                try
                {
                    callback.FileUpdate(file);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}