namespace ChatAPI.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public ContactRequest() { }

        public ContactRequest(int i_RequesterId, int i_RecipientId)
        {
            RequesterId = i_RequesterId;
            RecipientId = i_RecipientId;
        }
    }
}
