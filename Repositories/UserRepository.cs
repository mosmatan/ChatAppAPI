using ChatAPI.Data;
using ChatAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using ChatAPI.Exceptions;

namespace ChatAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext r_Context;

        public UserRepository(ChatDbContext i_Context)
        {
            r_Context = i_Context;
        }

        /// <summary>
        /// Retrieves a User entity by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to be retrieved.</param>
        /// <returns>A User entity if found, null otherwise.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user table is empty.</exception>
        public async Task<User?> GetUserByUsernameAsync(string i_Username)
        {
            if (r_Context.Users.Count() == 0)
            {
                throw new UserNotFoundException("Database is empty");
            }

            UserDTO? userDTO = await r_Context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == i_Username);

            if (userDTO == null)
            {
                return null;
            }

            User user = new User()
            {
                UserId = userDTO.UserId,
                Username = userDTO.Username,
                PasswordHash = userDTO.PasswordHash,
                FullName = userDTO.FullName
            };

            await addContactsFromUserDTOToUserAsync(userDTO, user);
            await addConverstionFromUserDTOToUserAsync(user, userDTO.ConversationIds);

            return user;
        }

        /// <summary>
        /// Adds contacts from UserDTO to User entity asynchronously.
        /// </summary>
        /// <param name="i_UserDTO">The UserDTO containing the contact IDs.</param>
        /// <param name="i_User">The User entity to add contacts to.</param>
        private async Task addContactsFromUserDTOToUserAsync(UserDTO i_UserDTO, User i_User)
        {
            foreach (int id in i_UserDTO.ContactIds)
            {
                var userDTO = await GetUserDTOByIdAsync(id);

                if (userDTO != null)
                {
                    i_User.AddContact(userDTO.ToUserContact());
                }
            }
        }

        /// <summary>
        /// Adds conversations from UserDTO to User entity asynchronously.
        /// </summary>
        /// <param name="i_User">The User entity to add conversations to.</param>
        /// <param name="i_ConversationsIds">The conversation IDs to add to the User.</param>
        private async Task addConverstionFromUserDTOToUserAsync(User i_User, IEnumerable<int> i_ConversationsIds)
        {
            IConverstionRepository converstionRepository = new ConverstionRepository(r_Context);

            i_User.Conversations = (await converstionRepository.GetAllConversationsWithIds(i_ConversationsIds)).ToList();
            i_User.Conversations.Sort();
            i_User.Conversations.Reverse();
        }

        /// <summary>
        /// Adds a new UserDTO entity to the database asynchronously.
        /// </summary>
        /// <param name="i_UserDTO">The UserDTO entity to add to the database.</param>
        public async Task AddUserAsync(UserDTO i_UserDTO)
        {
            await r_Context.Users.AddAsync(i_UserDTO);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a UserDTO entity by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to be retrieved.</param>
        /// <returns>A UserDTO entity if found, null otherwise.</returns>
        /// <exception cref="UserNotFoundException">Thrown if there are no users in the database.</exception>
        public async Task<UserDTO?> GetUserDTOByUsernameAsync(string i_Username)
        {
            if (!(await r_Context.Users.AnyAsync()))
            {
                return null;
            }

            UserDTO? userDTO = await r_Context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == i_Username);

            return userDTO;
        }

        /// <summary>
        /// Retrieves all UserDTO entities asynchronously.
        /// </summary>
        /// <returns>A list of all UserDTO entities.</returns>
        public async Task<IEnumerable<UserDTO>> GetAllUserDTOAsync()
        {
            var users = await r_Context.Users.ToListAsync();
            return users;
        }

        /// <summary>
        /// Retrieves all UserContact entities asynchronously.
        /// </summary>
        /// <returns>A list of all UserContact entities.</returns>
        public async Task<IEnumerable<UserContact>> GetAllUserContactAsync()
        {
            var usersDto = await GetAllUserDTOAsync();
            return usersDto.Select(us => us.ToUserContact());
        }

        /// <summary>
        /// Retrieves a UserContact entity by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to be retrieved.</param>
        /// <returns>A UserContact entity if found, null otherwise.</returns>
        public async Task<UserContact?> GetUserContactByUsernameAsync(string i_Username)
        {
            var userDto = await GetUserDTOByUsernameAsync(i_Username);
            return userDto?.ToUserContact();
        }

        /// <summary>
        /// Updates a UserDTO entity in the database asynchronously.
        /// </summary>
        /// <param name="i_UserDTO">The UserDTO entity to update.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task UpdateUserDTOAsync(UserDTO i_UserDTO)
        {
            r_Context.Update(i_UserDTO);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a UserDTO entity by user ID asynchronously.
        /// </summary>
        /// <param name="i_UserId">The user ID of the user to be retrieved.</param>
        /// <returns>A UserDTO entity if found, null otherwise.</returns>
        public async Task<UserDTO?> GetUserDTOByIdAsync(int i_UserId)
        {
            return await r_Context.Users.FindAsync(i_UserId);
        }

        /// <summary>
        /// Checks if a user ID exists in the database asynchronously.
        /// </summary>
        /// <param name="i_UserId">The user ID to check.</param>
        /// <returns>True if the user ID exists, false otherwise.</returns>
        public async Task<bool> IsUserIdExistAsync(int i_UserId)
        {
            var user = await r_Context.Users.FindAsync(i_UserId);
            return user != null;
        }

        /// <summary>
        /// Retrieves UserContact entities whose usernames start with a specified prefix asynchronously.
        /// </summary>
        /// <param name="i_StartWith">The prefix to filter usernames by.</param>
        /// <returns>A list of UserContact entities whose usernames start with the specified prefix.</returns>
        public async Task<IEnumerable<UserContact>> GetUserContactsWithUsernameStartWithAsync(string i_StartWith)
        {
            IEnumerable<UserContact> contacts = await r_Context.Users.AsNoTracking().
                Where(user => user.Username.StartsWith(i_StartWith)).
                Select(user => user.ToUserContact()).ToListAsync();

            return contacts;
        }

        /// <summary>
        /// Deletes a user by user ID asynchronously.
        /// </summary>
        /// <param name="i_UserId">The user ID of the user to be deleted.</param>
        public async Task DeleteUserByIdAsync(int i_UserId)
        {
            var user = await r_Context.Users.FindAsync(i_UserId);

            if (user == null)
            {
                return;
            }

            await removeUserFromContactsAsync(user.ContactIds, i_UserId);
            await removeUserFromConvesationsAsync(user.ConversationIds, i_UserId);

            r_Context.Users.Remove(user);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user from contact lists of other users asynchronously.
        /// </summary>
        /// <param name="i_ContactsId">The contact IDs to update.</param>
        /// <param name="i_UserId">The user ID to be removed from contacts.</param>
        private async Task removeUserFromContactsAsync(IEnumerable<int> i_ContactsId, int i_UserId)
        {
            foreach (int contactId in i_ContactsId)
            {
                var contact = await r_Context.Users.FindAsync(contactId);

                contact?.ContactIds.Remove(contactId);

                r_Context.Update(contact);
            }

            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user from conversations asynchronously.
        /// </summary>
        /// <param name="i_ConversationIds">The conversation IDs to update.</param>
        /// <param name="i_UserId">The user ID to be removed from conversations.</param>
        private async Task removeUserFromConvesationsAsync(IEnumerable<int> i_ConversationIds, int i_UserId)
        {
            foreach (var id in i_ConversationIds)
            {
                var conversation = await r_Context.Conversations.FindAsync(id);

                conversation.ParticipantIds.Remove(i_UserId);

                if (conversation.ParticipantIds.Count <= 1)
                {
                    foreach (var partiId in conversation.ParticipantIds)
                    {
                        var participant = await r_Context.Users.FindAsync(partiId);
                        participant?.ConversationIds.Remove(id);
                        r_Context.Users.Update(participant);
                    }

                    r_Context.Conversations.Remove(conversation);
                }
            }

            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a contact from a user's contact list and updates conversations asynchronously.
        /// </summary>
        /// <param name="i_User">The user from which the contact is to be removed.</param>
        /// <param name="i_ContactId">The contact ID to be removed.</param>
        public async Task RemoveContactAsync(UserDTO i_User, int i_ContactId)
        {
            i_User.ContactIds.Remove(i_ContactId);

            var contact = await r_Context.Users.FindAsync(i_ContactId);

            if (contact == null)
            {
                return;
            }

            contact.ContactIds.Remove(i_User.UserId);
            r_Context.Users.Update(contact);

            foreach (var conversationId in i_User.ConversationIds)
            {
                var conversation = await r_Context.Conversations.FindAsync(conversationId);

                if (conversation == null)
                {
                    i_User.ConversationIds.Remove(conversationId); // If conversation doesn't exist, remove it from list
                }

                // If the conversation is only between the two users, delete it
                if (conversation.ParticipantIds.Count == 2 &&
                    conversation.ParticipantIds.Contains(i_ContactId) &&
                    conversation.ParticipantIds.Contains(i_User.UserId))
                {
                    await deleteMessagesFromDatabaseAsync(conversation.MessageIds);
                    r_Context.Conversations.Remove(conversation);
                }
            }

            r_Context.Users.Update(i_User);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes messages from the database asynchronously.
        /// </summary>
        /// <param name="i_MessagesIds">The message IDs to be deleted.</param>
        private async Task deleteMessagesFromDatabaseAsync(IEnumerable<int> i_MessagesIds)
        {
            foreach (var id in i_MessagesIds)
            {
                var message = await r_Context.Massages.FindAsync(id);

                if (message != null)
                {
                    r_Context.Massages.Remove(message);
                }
            }

            await r_Context.SaveChangesAsync();
        }
    }
}
