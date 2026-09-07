using Chat;
using System.Collections.Generic;


namespace ChatServer
{
    //store all the chat data
    public static class Storage
    {
        public static List<User> Users =
            new List<User>();

        public static List<Channel> Channels =
            new List<Channel>();

        public static List<Message> Messages =
            new List<Message>();

        public static object LockObject =
            new object();

        public static List<PrivateChat> PrivateChats = new List<PrivateChat>();

        public static List<Notification> Notifications = new List<Notification>();
    }
}