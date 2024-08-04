namespace ChatAPI.Models.Requests
{
    public class MessageRequest
    {
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPassword { get; set; }
        public int ConversationId { get; set; }
        public string Content { get; set; }
    }
}
