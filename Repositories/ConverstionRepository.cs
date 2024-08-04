using ChatAPI.Data;
using ChatAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Repositories
{
    public class ConverstionRepository : IConverstionRepository
    {
        private readonly ChatDbContext r_Context;

        public ConverstionRepository(ChatDbContext i_Context)
        {
            r_Context = i_Context;
        }

        /// <summary>
        /// Adds a participant to a conversation asynchronously.
        /// </summary>
        /// <param name="i_ConversationId">The ID of the conversation.</param>
        /// <param name="i_ParticipentId">The ID of the participant to add.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the conversation or user is not found.</exception>
        public async Task AddParticipentAsync(int i_ConversationId, int i_ParticipentId)
        {
            var conversation = await r_Context.Conversations.FindAsync(i_ConversationId);
            var participent = await r_Context.Users.FindAsync(i_ParticipentId);

            if (conversation == null || participent == null)
            {
                throw new KeyNotFoundException("Conversation or User are not found");
            }

            conversation.ParticipantIds.Add(i_ParticipentId);
            participent.ConversationIds.Add(i_ConversationId);

            r_Context.Users.Update(participent);
            r_Context.Conversations.Update(conversation);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a new conversation asynchronously.
        /// </summary>
        /// <param name="i_ParticipentsIds">The IDs of the participants in the conversation.</param>
        public async Task CreateConverstionAsync(IEnumerable<int> i_ParticipentsIds)
        {
            var converstion = new ConversationDTO
            {
                StartTime = DateTime.Now,
                LastMessageTime = DateTime.Now,
                ParticipantIds = i_ParticipentsIds.ToList(),
                Name = string.Empty
            };

            await r_Context.Conversations.AddAsync(converstion);
            await r_Context.SaveChangesAsync();

            var participents = await r_Context.Users
                .Where(user => i_ParticipentsIds.Contains(user.UserId)).ToListAsync();

            foreach (var user in participents)
            {
                user.ConversationIds.Add(converstion.ConversationId);
                r_Context.Users.Update(user);
            }

            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all conversations of a user asynchronously.
        /// </summary>
        /// <param name="i_UserId">The ID of the user.</param>
        /// <returns>A list of ConversationDTO entities that the user is part of.</returns>
        public async Task<IEnumerable<ConversationDTO>> GetAllConverstionsOfUserAsync(int i_UserId)
        {
            var conversations = await r_Context.Conversations
                .Where(con => con.ParticipantIds.Contains(i_UserId)).ToListAsync();

            return conversations;
        }

        /// <summary>
        /// Retrieves a ConversationDTO entity by conversation ID asynchronously.
        /// </summary>
        /// <param name="i_ConversationId">The ID of the conversation to retrieve.</param>
        /// <returns>A ConversationDTO entity if found, null otherwise.</returns>
        public async Task<ConversationDTO?> GetConversationDTOByIdAsync(int i_ConversationId)
        {
            return await r_Context.Conversations.FindAsync(i_ConversationId);
        }

        /// <summary>
        /// Retrieves a Conversation entity by conversation ID asynchronously.
        /// </summary>
        /// <param name="i_ConversationId">The ID of the conversation to retrieve.</param>
        /// <returns>A Conversation entity if found, null otherwise.</returns>
        public async Task<Conversation?> GetConversationByIdAsync(int i_ConversationId)
        {
            var conversationDTO = await GetConversationDTOByIdAsync(i_ConversationId);

            if (conversationDTO == null)
            {
                return null;
            }

            var conversation = new Conversation
            {
                ConversationId = conversationDTO.ConversationId,
                LastMessageTime = conversationDTO.LastMessageTime,
                StartTime = conversationDTO.StartTime,
                Participants = await getConversationParticipents(conversationDTO.ParticipantIds),
                Messages = await getConversationMessages(conversationDTO.MessageIds)
            };

            return conversation;
        }

        /// <summary>
        /// Retrieves the participants of a conversation by their IDs asynchronously.
        /// </summary>
        /// <param name="i_ParticipentsId">The IDs of the participants.</param>
        /// <returns>A collection of UserContact entities representing the participants.</returns>
        private async Task<ICollection<UserContact>> getConversationParticipents(IEnumerable<int> i_ParticipentsId)
        {
            return await r_Context.Users.AsNoTracking()
                .Where(per => i_ParticipentsId.Contains(per.UserId))
                .Select(per => per.ToUserContact()).ToListAsync();
        }

        /// <summary>
        /// Retrieves the messages of a conversation by their IDs asynchronously.
        /// </summary>
        /// <param name="i_MessagesId">The IDs of the messages.</param>
        /// <returns>A collection of MessageDTO entities representing the messages.</returns>
        private async Task<ICollection<MessageDTO>> getConversationMessages(IEnumerable<int> i_MessagesId)
        {
            return await r_Context.Massages.AsNoTracking()
                .Where(message => i_MessagesId.Contains(message.MessageId)).ToListAsync();
        }

        /// <summary>
        /// Retrieves all conversations with specified IDs asynchronously.
        /// </summary>
        /// <param name="i_ConversationsId">The IDs of the conversations to retrieve.</param>
        /// <returns>A collection of ConversationDTO entities representing the conversations.</returns>
        public async Task<ICollection<ConversationDTO>> GetAllConversationsWithIds(IEnumerable<int> i_ConversationsId)
        {
            return await r_Context.Conversations.AsNoTracking()
                .Where(con => i_ConversationsId.Contains(con.ConversationId)).ToListAsync();
        }
    }
}
