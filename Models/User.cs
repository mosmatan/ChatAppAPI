using System.Text;
using System.Security.Cryptography;

namespace ChatAPI.Models
{
    public class User
    {
        private ICollection<UserContact> m_Contacts = new List<UserContact>();

        public int UserId { get; set; } = 0;
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public List<UserContact> Contacts { 
            get 
            { 
                UserContact[] result = new UserContact[m_Contacts.Count];
                m_Contacts.CopyTo(result, 0);

                return result.ToList();
            }
        } 
        public List<ConversationDTO> Conversations { get; set; } = new List<ConversationDTO>();

        public UserContact ToUserContact()
        {
            UserContact contact = new UserContact() { UserId = this.UserId, Username = this.Username, FullName = this.FullName };

            return contact;
        }

        public void AddContact(UserContact i_Contact)
        {
            m_Contacts.Add(i_Contact);
        }

        public void RemoveContact(UserContact i_Contact)
        {
            m_Contacts.Remove(i_Contact);
        }

        public bool VerifyPassword(string enteredPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var enteredHash = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword))).Replace("-", "").ToLower();
                return enteredHash == PasswordHash;
            }
        }
    }
}
