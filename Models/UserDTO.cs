using System.Text;
using System.Security.Cryptography;

namespace ChatAPI.Models
{
    public class UserDTO
    {
        public int UserId { get; set; }  // Unique identifier for the user
        public string Username { get; set; }  // User's chosen login name
        public string PasswordHash { get; set; }  // Hashed version of the user's password
        public string FullName { get; set; }  // User's full name for display purposes

        // Lists of IDs representing relationships
        public List<int> ContactIds { get; set; } = new List<int>();  // IDs of contacts
        public List<int> ConversationIds { get; set; } = new List<int>();  // IDs of conversations

        public UserContact ToUserContact()
        {
            UserContact contact = new UserContact() { UserId = this.UserId, Username = this.Username, FullName = this.FullName };
            return contact;
        }

        public void AddContactId(int i_ContactId)
        {
            ContactIds.Add(i_ContactId);
        }

        public bool VerifyPassword(string enteredPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var enteredHash = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword))).Replace("-", "").ToLower();
                return (enteredHash == PasswordHash || enteredPassword == PasswordHash);
            }
        }
    }
}
