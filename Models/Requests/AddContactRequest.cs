namespace ChatAPI.Models.Requests
{
    public class AddContactRequest
    {
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public string RequesterPassword { get; set; }
    }
}
