namespace Chat
{
    public class SharedFile
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string sharedBy { get; set; }
        public byte[] data { get; set; }
        public string channelName { get; set; }

        public SharedFile()
        {
            sharedBy = "";
            data = null;
        }
    }
}
