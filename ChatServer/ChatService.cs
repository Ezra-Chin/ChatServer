using System;
using System.Collections.Generic;
using System.Linq;
using Chat;
using ChatContract;

namespace ChatServer
{
    public class ChatService : IChatService
    {


        public void SignIn(string userId)
        {

            lock (Storage.LockObject)
            {

                if (Storage.Users.Any(
                    x => x.userId == userId))
                {
                    return;
                }


                Storage.Users.Add(
                    new User
                    {
                        userId = userId
                    });

            }

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



        public void CreateChannel(
            string userId,
            string channelName)
        {

            lock (Storage.LockObject)
            {

                if (Storage.Channels.Any(
                    x => x.channelName == channelName))
                {
                    return;
                }


                Storage.Channels.Add(
                    new Channel
                    {
                        channelName = channelName
                    });

            }

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
            string message)
        {

            User user =
            Storage.Users.FirstOrDefault(
                x => x.userId == userId);


            if (user == null)
                return;


            Storage.Messages.Add(
                new Message
                {
                    sender = userId,
                    text = message,
                    time = DateTime.Now
                });

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



            Storage.Messages.Add(
                new Message
                {
                    sender = senderId,
                    text = message,
                    time = DateTime.Now
                });

        }




        public SharedFile ShareFile(
            string userId,
            string fileName,
            byte[] data)
        {

            if (data.Length > 2 * 1024 * 1024)
                return null;



            SharedFile file =
            new SharedFile
            {
                fileName = fileName,
                sharedBy = userId,
                data = data
            };



            return file;

        }


    }
}