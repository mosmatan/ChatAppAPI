using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ChatAPI.Models.Requests
{
    public class ContactRequestAnswer
    {
        public int RequestId {  get; set; }
        public string Password {  get; set; }
        public bool IsAprroved { get; set; }
    }
}
