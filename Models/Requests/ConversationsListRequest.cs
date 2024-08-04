using System.Reflection.Metadata.Ecma335;

namespace ChatAPI.Models.Requests
{
    public class ConversationsListRequest
    {
        public string Password { get; set; }
        public List<int> ConversationsId { get; set; }
    }
}
