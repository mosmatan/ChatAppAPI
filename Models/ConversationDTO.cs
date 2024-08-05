namespace ChatAPI.Models
{
    public class ConversationDTO : IComparable<ConversationDTO>
    {
        public int ConversationId { get; set; }
        public string Name { get; set; }
        public List<int> ParticipantIds { get; set; } = new List<int>();  // IDs of participants
        public List<int> MessageIds { get; set; } = new List<int>();  // IDs of messages
        public DateTime StartTime { get; set; }  // When the conversation started
        public DateTime LastMessageTime { get; set; }  // When the last message was sent

        public int CompareTo(ConversationDTO? other)
        {
            if( other == null)
            {
                return 0;
            }
            return this.LastMessageTime.CompareTo(other.LastMessageTime);
        }
    }
}
