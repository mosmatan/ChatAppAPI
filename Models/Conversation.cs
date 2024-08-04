namespace ChatAPI.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public string Name { get; set; }
        public ICollection<UserContact> Participants { get; set; } = new List<UserContact>();  // Users involved in the conversation
        public ICollection<MessageDTO> Messages { get; set; } = new List<MessageDTO>();  // Messages in the conversation
        public DateTime StartTime { get; set; }  // When the conversation started
        public DateTime LastMessageTime { get; set; }  // When the last message was sent
    }
}
