using ChatAPI.Exceptions;
using ChatAPI.Models;
using ChatAPI.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ChatAPI.Services
{
    public class ConversationService
    {
        private readonly IConverstionRepository r_ConverstionRepository;
        private readonly IMessageRepository r_MessageRepository;
        private readonly IUserRepository r_UserRepository;

        public ConversationService(IConverstionRepository i_ConverstionRepository,
            IMessageRepository i_MessageRepository, IUserRepository i_UserRepository)
        {
            r_ConverstionRepository = i_ConverstionRepository;
            r_MessageRepository = i_MessageRepository;
            r_UserRepository = i_UserRepository;
        }

        /// <summary>
        /// Retrieves a ConversationDTO entity by conversation ID, sender ID, and sender password asynchronously.
        /// </summary>
        /// <param name="i_ConversationID">The ID of the conversation.</param>
        /// <param name="i_SenderId">The ID of the sender.</param>
        /// <param name="i_SenderPassword">The password of the sender.</param>
        /// <returns>The ConversationDTO entity if found and authenticated.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the conversation or sender is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the sender is not in the conversation or the password is invalid.</exception>
        private async Task<ConversationDTO> getConversationDTOAsync(int i_ConversationID, int i_SenderId, string i_SenderPassword)
        {
            var conversDTO = await r_ConverstionRepository.GetConversationDTOByIdAsync(i_ConversationID);
            var sender = await r_UserRepository.GetUserDTOByIdAsync(i_SenderId);

            if (conversDTO == null || sender == null)
            {
                throw new EntityNotFoundException("Conversation or sender not found");
            }
            else if (!conversDTO.ParticipantIds.Contains(sender.UserId))
            {
                throw new AuthenticateException("The sender is not in this conversation");
            }
            else if (sender.PasswordHash != i_SenderPassword)
            {
                throw new AuthenticateException("Invalid Password");
            }

            return conversDTO;
        }

        /// <summary>
        /// Sends a message in a conversation asynchronously.
        /// </summary>
        /// <param name="i_ConversationID">The ID of the conversation.</param>
        /// <param name="i_SenderId">The ID of the sender.</param>
        /// <param name="i_SenderName">The name of the sender.</param>
        /// <param name="i_SenderPassword">The password of the sender.</param>
        /// <param name="i_WhenSet">The timestamp when the message was sent.</param>
        /// <param name="i_Content">The content of the message.</param>
        public async Task SendMessageAsync(int i_ConversationID, int i_SenderId, string i_SenderName, string i_SenderPassword, DateTime i_WhenSet, string i_Content)
        {
            try
            {
                var conversationDTO = await getConversationDTOAsync(i_ConversationID, i_SenderId, i_SenderPassword);
                await r_MessageRepository.CreateMessageAsync(conversationDTO, i_SenderId, i_SenderName, i_WhenSet, i_Content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a conversation by ID asynchronously.
        /// </summary>
        /// <param name="i_ConversationID">The ID of the conversation.</param>
        /// <param name="i_UserId">The ID of the user.</param>
        /// <param name="i_Password">The password of the user.</param>
        /// <returns>The Conversation entity if found and authenticated.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the conversation is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the user is not in the conversation or the password is invalid.</exception>
        public async Task<Conversation?> GetConversationAsync(int i_ConversationID, int i_UserId, string i_Password)
        {
            try
            {
                await getConversationDTOAsync(i_ConversationID, i_UserId, i_Password); // Check if conversation exists and is allowed for the user

                var convers = await r_ConverstionRepository.GetConversationByIdAsync(i_ConversationID);

                if (convers == null)
                {
                    throw new EntityNotFoundException($"Conversation with id {i_ConversationID} not found");
                }

                return convers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the chosen user's conversations asynchronously.
        /// </summary>
        /// <param name="i_UserId">The ID of the user.</param>
        /// <param name="i_Password">The password of the user.</param>
        /// <param name="i_ConversationsId">The IDs of the conversations to retrieve.</param>
        /// <returns>A list of ConversationDTO entities that the user is part of.</returns>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if any of the conversation IDs are invalid.</exception>
        public async Task<IEnumerable<ConversationDTO>> GetChoosenUserConverstionsAsync(int i_UserId, string i_Password, IEnumerable<int> i_ConversationsId)
        {
            ICollection<ConversationDTO> conversations;

            var user = await r_UserRepository.GetUserDTOByIdAsync(i_UserId);

            if (user.PasswordHash != i_Password)
            {
                throw new AuthenticateException();
            }

            foreach (int conversationId in i_ConversationsId)
            {
                if (!user.ConversationIds.Contains(conversationId))
                {
                    throw new IndexOutOfRangeException($"Invalid conversation id {conversationId}");
                }
            }

            if (i_ConversationsId == null)
            {
                conversations = await r_ConverstionRepository.GetAllConversationsWithIds(user.ConversationIds);
            }
            else
            {
                conversations = await r_ConverstionRepository.GetAllConversationsWithIds(i_ConversationsId);
            }

            (conversations as List<ConversationDTO>).Sort();
            (conversations as List<ConversationDTO>).Reverse();

            return conversations;
        }
    }
}
