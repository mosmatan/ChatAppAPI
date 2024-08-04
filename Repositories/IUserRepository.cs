using ChatAPI.Models;

namespace ChatAPI.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a User entity by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to be retrieved.</param>
        /// <returns>A User entity if found, null otherwise.</returns>
        Task<User?> GetUserByUsernameAsync(string i_Username);

        /// <summary>
        /// Retrieves a UserDTO entity by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to be retrieved.</param>
        /// <returns>A UserDTO entity if found, null otherwise.</returns>
        Task<UserDTO?> GetUserDTOByUsernameAsync(string i_Username);

        /// <summary>
        /// Retrieves a UserDTO entity by user ID asynchronously.
        /// </summary>
        /// <param name="i_UserId">The user ID of the user to be retrieved.</param>
        /// <returns>A UserDTO entity if found, null otherwise.</returns>
        Task<UserDTO?> GetUserDTOByIdAsync(int i_UserId);

        /// <summary>
        /// Adds a new UserDTO entity to the database asynchronously.
        /// </summary>
        /// <param name="i_UserDTO">The UserDTO entity to add to the database.</param>
        Task AddUserAsync(UserDTO i_UserDTO);

        /// <summary>
        /// Retrieves all UserDTO entities asynchronously.
        /// </summary>
        /// <returns>A list of all UserDTO entities.</returns>
        Task<IEnumerable<UserDTO>> GetAllUserDTOAsync();

        /// <summary>
        /// Retrieves all UserContact entities asynchronously.
        /// </summary>
        /// <returns>A list of all UserContact entities.</returns>
        Task<IEnumerable<UserContact>> GetAllUserContactAsync();

        /// <summary>
        /// Retrieves a UserContact entity by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to be retrieved.</param>
        /// <returns>A UserContact entity if found, null otherwise.</returns>
        Task<UserContact?> GetUserContactByUsernameAsync(string i_Username);

        /// <summary>
        /// Retrieves UserContact entities whose usernames start with a specified prefix asynchronously.
        /// </summary>
        /// <param name="i_StartWith">The prefix to filter usernames by.</param>
        /// <returns>A list of UserContact entities whose usernames start with the specified prefix.</returns>
        Task<IEnumerable<UserContact>> GetUserContactsWithUsernameStartWithAsync(string i_StartWith);

        /// <summary>
        /// Updates a UserDTO entity in the database asynchronously.
        /// </summary>
        /// <param name="i_UserDTO">The UserDTO entity to update.</param>
        Task UpdateUserDTOAsync(UserDTO i_UserDTO);

        /// <summary>
        /// Checks if a user ID exists in the database asynchronously.
        /// </summary>
        /// <param name="i_UserId">The user ID to check.</param>
        /// <returns>True if the user ID exists, false otherwise.</returns>
        Task<bool> IsUserIdExistAsync(int i_UserId);

        /// <summary>
        /// Deletes a user by user ID asynchronously.
        /// </summary>
        /// <param name="i_UserId">The user ID of the user to be deleted.</param>
        Task DeleteUserByIdAsync(int i_UserId);

        /// <summary>
        /// Removes a contact from a user's contact list and updates conversations asynchronously.
        /// </summary>
        /// <param name="i_User">The user from which the contact is to be removed.</param>
        /// <param name="i_ContactId">The contact ID to be removed.</param>
        Task RemoveContactAsync(UserDTO i_User, int i_ContactId);
    }
}
