using ChatAPI.Models;

namespace ChatAPI.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; } // Navigation property
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } // Navigation property
        public DateTime Timestamp { get; set; }
    }
}
