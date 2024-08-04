using ChatAPI.Models;

namespace ChatAPI.Repositories
{
    public interface IConverstionRepository
    {
        /// <summary>
        /// Creates a new conversation asynchronously.
        /// </summary>
        /// <param name="i_PaticipentsIds">The IDs of the participants in the conversation.</param>
        Task CreateConverstionAsync(IEnumerable<int> i_PaticipentsIds);

        /// <summary>
        /// Adds a participant to a conversation asynchronously.
        /// </summary>
        /// <param name="i_ConversionId">The ID of the conversation.</param>
        /// <param name="i_PaticipentId">The ID of the participant to add.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the conversation or user is not found.</exception>
        Task AddParticipentAsync(int i_ConversionId, int i_PaticipentId);

        /// <summary>
        /// Retrieves all conversations of a user asynchronously.
        /// </summary>
        /// <param name="i_ConversionId">The ID of the user.</param>
        /// <returns>A list of ConversationDTO entities that the user is part of.</returns>
        Task<IEnumerable<ConversationDTO>> GetAllConverstionsOfUserAsync(int i_ConversionId);

        /// <summary>
        /// Retrieves a ConversationDTO entity by conversation ID asynchronously.
        /// </summary>
        /// <param name="i_ConversationId">The ID of the conversation to retrieve.</param>
        /// <returns>A ConversationDTO entity if found, null otherwise.</returns>
        Task<ConversationDTO?> GetConversationDTOByIdAsync(int i_ConversationId);

        /// <summary>
        /// Retrieves a Conversation entity by conversation ID asynchronously.
        /// </summary>
        /// <param name="i_ConversationId">The ID of the conversation to retrieve.</param>
        /// <returns>A Conversation entity if found, null otherwise.</returns>
        Task<Conversation?> GetConversationByIdAsync(int i_ConversationId);

        /// <summary>
        /// Retrieves all conversations with specified IDs asynchronously.
        /// </summary>
        /// <param name="i_ConversationsId">The IDs of the conversations to retrieve.</param>
        /// <returns>A collection of ConversationDTO entities representing the conversations.</returns>
        Task<ICollection<ConversationDTO>> GetAllConversationsWithIds(IEnumerable<int> i_ConversationsId);
    }
}
