using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Chat;
using ChatContract;

namespace ChatServer
{
    public class ChatService : IChatService , IPollingChatService
    {


        public bool SignIn(string userId)
        {

            lock (Storage.LockObject)
            {

                if (Storage.Users.Any(
                    x => x.userId == userId))
                {
                    return false;
                }


                Storage.Users.Add(
                    new User
                    {
                        userId = userId
                    });

            }
            return true;

        }



        public void SignOut(string userId)
        {

            lock (Storage.LockObject)
            {

                User user =
                Storage.Users.FirstOrDefault(
                    x => x.userId == userId);


                if (user != null)
                {

                    LeaveChannel(userId);

                    Storage.Users.Remove(user);

                }

            }

        }



        public List<Channel> GetChannels()
        {
            return Storage.Channels;
        }



        public bool CreateChannel(
            string userId,
            string channelName)
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
            return true;

        }



        public void JoinChannel(
            string userId,
            string channelName)
        {

            lock (Storage.LockObject)
            {

                Channel channel =
                Storage.Channels.FirstOrDefault(
                    x => x.channelName == channelName);


                if (channel == null)
                    return;



                foreach (Channel c in Storage.Channels)
                {
                    c.members.Remove(userId);
                }



                channel.members.Add(userId);



                User user =
                Storage.Users.First(
                    x => x.userId == userId);


                user.currentChannel =
                    channelName;

                user.joinedChannelAt = DateTime.Now;

            }

        }



        public void LeaveChannel(
            string userId)
        {

            foreach (Channel c in Storage.Channels)
            {
                c.members.Remove(userId);
            }



            User user =
            Storage.Users.FirstOrDefault(
                x => x.userId == userId);


            if (user != null)
            {
                user.currentChannel = null;
            }

        }



        public void SendMessage(
            string userId,
            string channelName,
            string message)
        {
            User user =
            Storage.Users.FirstOrDefault(
                x => x.userId == userId);


            if (user == null)
                return;





            Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName.Equals(channelName));
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


        public List<Message> GetPrivateMessages(string senderId, string recipientId)
        {
            PrivateChat privateChat =
                  Storage.PrivateChats.FirstOrDefault(x => (x.userId1 == senderId && x.userId2 == recipientId) || (x.userId1 == recipientId && x.userId2 == senderId));
            if (privateChat == null)
            {
                return new List<Message>();
            }
            return privateChat.messages.OrderBy( x => x.time).ToList();
        }
        public void SendPrivateMessage(
            string senderId,
            string recipientId,
            string message)
        {

            User sender =
            Storage.Users.FirstOrDefault(
                x => x.userId == senderId);


            User receiver =
            Storage.Users.FirstOrDefault(
                x => x.userId == recipientId);



            if (sender == null || receiver == null)
                return;



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
        }

        public List<Notification> GetNotifications(string userId)
        {
            lock (Storage.LockObject)
            {
                return Storage.Notifications.Where(x => x.recipient == userId && !x.read).ToList();
            }
        }

        public Channel GetChannel(string channelName, string userId)
        {
            lock (Storage.LockObject)
            {
                Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName == channelName);



                if (channel == null)
                    return null;

                User user = Storage.Users.FirstOrDefault(x => x.userId == userId);



                if (user == null) return null;


                Channel result = new Channel
                {
                    channelName = channel.channelName,
                    members = new List<string>(channel.members),
                    files = new List<SharedFile>(channel.files),
                    messages = channel.messages.Where(x => x.time >= user.joinedChannelAt).ToList()

                };

                return result;
            }
        }
        public void MarkNotificationAsRead(Notification n)
        {
            string recipient = n.recipient;
            string sender = n.sender;
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

            if (data.Length > 2 * 1024 * 1024)
                return null;

            Channel channel = Storage.Channels.FirstOrDefault(x => x.channelName.Equals(channelName));
            if (channel == null)
            {
                return null;
            }

            SharedFile file =
            new SharedFile
            {
                fileName = fileName,
                sharedBy = userId,
                data = data
            };

            channel.files.Add(file);

            return file;

        }


    }
}