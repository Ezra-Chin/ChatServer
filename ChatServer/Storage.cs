using System.Collections.Generic;
using Chat;


namespace ChatServer
{
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

    }
}