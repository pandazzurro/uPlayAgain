namespace uPlayAgain.Models
{
    public class MessageCountResponse
    {
        public int Incoming { get; set; }
        public int Outgoing { get; set; }
        public int Transactions { get; set; }
        public int LibrariesComponents { get; set; }
    }
}
