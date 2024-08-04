using ChatAPI.Data;
using ChatAPI.Models;

namespace ChatAPI.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDbContext r_Context;

        public MessageRepository(ChatDbContext i_Context)
        {
            r_Context = i_Context;
        }

        /// <summary>
        /// Creates a new message and adds it to the specified conversation asynchronously.
        /// </summary>
        /// <param name="i_Conversation">The conversation to which the message belongs.</param>
        /// <param name="i_SenderId">The ID of the sender.</param>
        /// <param name="i_SenderName">The username of the sender.</param>
        /// <param name="i_WhenSent">The timestamp of when the message was sent.</param>
        /// <param name="i_Content">The content of the message.</param>
        public async Task CreateMessageAsync(ConversationDTO i_Conversation, int i_SenderId, string i_SenderName, DateTime i_WhenSent, string i_Content)
        {
            var message = new MessageDTO
            {
                ConversationId = i_Conversation.ConversationId,
                SenderId = i_SenderId,
                SenderUsername = i_SenderName,
                Timestamp = i_WhenSent,
                Content = i_Content
            };

            await r_Context.Massages.AddAsync(message);
            await r_Context.SaveChangesAsync();

            i_Conversation.MessageIds.Add(message.MessageId);
            i_Conversation.LastMessageTime = message.Timestamp;
            r_Context.Update(i_Conversation);
            await r_Context.SaveChangesAsync();
        }
    }
}
