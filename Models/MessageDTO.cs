namespace ChatAPI.Models
{
    public class MessageDTO
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public int ConversationId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
