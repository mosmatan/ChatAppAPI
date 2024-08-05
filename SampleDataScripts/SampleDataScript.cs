using ChatAPI.Models;
using ChatAPI.Models.Requests;
using Microsoft.AspNetCore.Identity.Data;

namespace ChatAPI.SampleDataScripts
{
    public class SampleDataScript
    {
        private readonly string apiUrl = "https://localhost:7235/api/";

        private readonly HttpClient client = new HttpClient();
        User[] users = new User[10];
        private readonly string[] usernames = { "Matan", "Niv", "Ziv", "Daniel", "Jonathan", "Tamir", "Gal", "Guy", "Noy", "Dana" };
        private readonly string[] names = { "Matan M", "Niv N", "Ziv Z", "Daniel D", "Jonathan J", "Tamir T", "Gal G", "Guy G", "Noy N", "Dana D" };

        private readonly (int from, int to)[] connectionArr = { (0, 2), (0, 3), (0, 6), (1, 7), (1, 9), (2, 3), (2, 5), (2, 8), (3, 5), (3, 7), (3, 8), (4, 5), (4, 6) };


        public async Task InitializeSampleData()
        {
            for (int i = 0; i < users.Length; i++)
            {
                await initializeUser(i);
            }

            await connectUsers();
        }

        private async Task initializeUser(int i_Index)
        {
            RegisterUserRequest register = new RegisterUserRequest() { 
                Username=usernames[i_Index],
                Password="1234",
                FullName=names[i_Index]
            };

            var response = await client.PostAsJsonAsync($"{apiUrl}User/register", register);

            if (response.IsSuccessStatusCode)
            {
                users[i_Index] = await response.Content.ReadFromJsonAsync<User>();
            }
            
        }

        private async Task connectUsers()
        {
            for(int i = 0; i < connectionArr.Length; i++)
            {
                var ids = connectionArr[i];
                await sendAndAcceptContactRequest(users[ids.from], users[ids.to]);
            }
        }

        private async Task sendAndAcceptContactRequest(User i_From, User i_To)
        {
            AddContactRequest contact = new AddContactRequest()
            {
                RequesterId = i_From.UserId,
                RecipientId = i_To.UserId,
                RequesterPassword = i_From.PasswordHash
            };

            var response = await client.PostAsJsonAsync($"{apiUrl}User/contact/sendcontactrequest", contact);

            if (response.IsSuccessStatusCode)
            {
                ContactRequest contactRequest = await response.Content.ReadFromJsonAsync<ContactRequest>();

                ContactRequestAnswer answer = new ContactRequestAnswer()
                {
                    RequestId = contactRequest.Id,
                    Password = i_From.PasswordHash,
                    IsAprroved = true
                };

                response = await client.PutAsJsonAsync($"{apiUrl}User/contact/responsecontactrequest", answer);
            }
        }
    }
}
