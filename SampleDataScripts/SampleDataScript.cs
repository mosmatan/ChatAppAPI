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
        private readonly string[] usernames = { "Matan", "Niv", "Ziv", "Daniel", "Jonathan", "Tamir", "Gal", "Yossi", "Noy", "Dana" };
        private readonly string[] names = { "Matan M", "Niv N", "Ziv Z", "Daniel D", "Jonathan J", "Tamir T", "Gal G", "Yossi Y", "Noy N", "Dana D" };

        private readonly (int from, int to)[] connectionArr = { (0, 2), (0, 3), (0, 6), (1, 7), (1, 9), (2, 3), (2, 5), (2, 8), (3, 5), (3, 7), (3, 8), (4, 5), (4, 6) };

        private readonly string[,] conversationsScripts = new string[,]
            {
                {
                    "Hello, how can I help you today?",
                    "Hi, I need some information about your services.",
                    "Sure, what specific information are you looking for?",
                    "I'm interested in learning more about your pricing.",
                    "Our pricing depends on the services you choose. Can you tell me which services you're interested in?",
                    "I'm looking at your premium package."
                },
                {
                    "Good afternoon, how can I assist you?",
                    "Hi, I have a question about my recent order.",
                    "Of course, can you provide your order number?",
                    "Yes, it's 12345. I need to know the status.",
                    "Let me check that for you. Please hold on a moment.",
                    "Thank you."
                },
                {
                    "Hi there, what can I do for you today?",
                    "Hello, I need technical support for my device.",
                    "I'd be happy to help. What's the issue you're experiencing?",
                    "My device won't turn on.",
                    "Have you tried charging it?",
                    "Yes, but it still won't turn on."
                },
                {
                    "Hello, thank you for calling. How can I assist you?",
                    "Hi, I'm calling to inquire about your return policy.",
                    "Our return policy allows returns within 30 days of purchase.",
                    "Great, thank you for the information.",
                    "Is there anything else I can help you with?",
                    "No, that's all. Thanks!"
                },
                {
                    "Hi, how may I help you today?",
                    "Hello, I have a billing question.",
                    "Sure, can you specify what your question is about?",
                    "I noticed an unexpected charge on my statement.",
                    "Can you provide the date and amount of the charge?",
                    "It's from last week, and the amount is $50."
                },
                {
                    "Good morning, what can I help you with today?",
                    "Hi, I'm looking for information on your latest products.",
                    "Certainly, we have a new range of products. Any specific type you're interested in?",
                    "Yes, I'm particularly interested in your new laptops.",
                    "We have several models available. Do you have any preferences in terms of specifications?",
                    "I'm looking for something with a lot of storage and a fast processor."
                },
                {
                    "Hi, do you have any recommendations for a good book?",
                    "Yes, I recently read a great mystery novel.",
                    "That sounds interesting. What's the title?",
                    "It's called 'The Silent Patient'.",
                    "I'll check it out. Thanks for the recommendation.",
                    "You're welcome. I hope you enjoy it."
                },
                {
                    "Hello, do you know any good recipes for dinner?",
                    "I love making pasta dishes. How about a classic carbonara?",
                    "That sounds delicious. Can you share the recipe?",
                    "Sure, you'll need eggs, cheese, pancetta, and spaghetti.",
                    "Great, I'll give it a try tonight.",
                    "Let me know how it turns out!"
                },
                {
                    "Hi, have you seen any good movies lately?",
                    "Yes, I watched a fantastic sci-fi film recently.",
                    "What's the name of the movie?",
                    "It's called 'Interstellar'.",
                    "I've heard good things about that one. I'll watch it soon.",
                    "You won't be disappointed!"
                },
                {
                    "Hello, do you have any travel plans for the holidays?",
                    "Yes, I'm planning a trip to the mountains.",
                    "That sounds exciting. Which mountain range are you visiting?",
                    "I'm heading to the Rockies.",
                    "I've always wanted to visit there. Have a great trip!",
                    "Thank you! I'm really looking forward to it."
                },
                {
                    "Hi, are you following any sports events this season?",
                    "Yes, I'm really into the football championship right now.",
                    "Which team are you rooting for?",
                    "I'm supporting the local team.",
                    "They've been playing really well this season.",
                    "I agree. I'm hopeful they'll win the championship."
                },
                {
                    "Hello, have you tried any new hobbies recently?",
                    "Yes, I've started learning how to paint.",
                    "That sounds wonderful. What kind of painting are you doing?",
                    "I'm experimenting with watercolors.",
                    "Watercolors are beautiful. Do you have any tips for beginners?",
                    "Just practice a lot and don't be afraid to make mistakes."
                },
                {
                    "Hi, have you picked up any new skills lately?",
                    "Yes, I've been learning how to play the guitar.",
                    "That's great! How's it going so far?",
                    "It's challenging, but I'm enjoying it.",
                    "Do you have any favorite songs you like to play?",
                    "I'm currently working on 'Wonderwall' by Oasis."
                }
            };


        public async Task InitializeSampleData()
        {
            for (int i = 0; i < users.Length; i++)
            {
                await initializeUser(i);
            }

            await connectUsers();
            await sendAllConversations();
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

        private async Task sendAllConversations()
        {
            for (int i = 0; i < connectionArr.Length; i++)
            {
                await sendConversationScript(i, i + 1, users[connectionArr[i].from], users[connectionArr[i].to]);
            }
        }

        private async Task sendConversationScript(int i_Index, int i_ConversationId, User i_User1, User i_User2)
        {
            User[] talkers = { i_User1, i_User2 };

            MessageRequest message = new MessageRequest();
            message.ConversationId = i_ConversationId;
            
            for (int i = 0; i < 6; i++)
            {
                message.SenderId = talkers[i % 2].UserId;
                message.SenderUsername = talkers[i % 2].Username;
                message.SenderPassword = talkers[i % 2].PasswordHash;
                message.Content = conversationsScripts[i_Index, i];

                await client.PostAsJsonAsync($"{apiUrl}Conversation/sendmessage", message);
            }
        }
    }
}
