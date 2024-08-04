using ChatAPI.Models;

namespace ChatAPI.Repositories
{
    public interface IMessageRepository
    {
        /// <summary>
        /// Creates a new message and adds it to the specified conversation asynchronously.
        /// </summary>
        /// <param name="i_Conversation">The conversation to which the message belongs.</param>
        /// <param name="i_SenderId">The ID of the sender.</param>
        /// <param name="i_SenderName">The username of the sender.</param>
        /// <param name="i_WhenSent">The timestamp of when the message was sent.</param>
        /// <param name="i_Content">The content of the message.</param>
        Task CreateMessageAsync(ConversationDTO i_Conversation, int i_SenderId, string i_SenderName, DateTime i_WhenSent, string i_Content);
    }
}
