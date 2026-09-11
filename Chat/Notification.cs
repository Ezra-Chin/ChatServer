namespace Chat
{
    public class Notification
    {
        public string recipient { set; get; }
        public string sender { set; get; }
        public Message message { set; get; }
        public bool read { get; set; }

        public Notification()
        {}
        
        public Notification(string recipient, string sender, Message message, bool read)
        {
            this.recipient = recipient;
            this.sender = sender;
            this.message = message;
            this.read = read;
        }
    }
}
