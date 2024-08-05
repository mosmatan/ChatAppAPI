using ChatAPI.Exceptions;
using ChatAPI.Models;
using ChatAPI.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ChatAPI.Services
{
    public class UserService
    {
        private readonly IUserRepository r_UserRepository;
        private readonly IContactRequestRepository r_ContactRequestRepository;
        private readonly IConverstionRepository r_ConverstionRepository;

        public UserService(IUserRepository userRepository,
            IContactRequestRepository contactRequestRepository,
            IConverstionRepository i_ConverstionRepository)
        {
            r_UserRepository = userRepository;
            r_ContactRequestRepository = contactRequestRepository;
            r_ConverstionRepository = i_ConverstionRepository;
        }

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the new user.</param>
        /// <param name="i_Password">The password of the new user.</param>
        /// <param name="i_FullName">The full name of the new user.</param>
        /// <returns>The registered User entity.</returns>
        /// <exception cref="Exception">Thrown if the username already exists.</exception>
        public async Task<User> RegisterUserAsync(string i_Username, string i_Password, string i_FullName)
        {

            var existingUser = await r_UserRepository.GetUserDTOByUsernameAsync(i_Username);

            if (existingUser != null)
            {
                throw new Exception("Username is already exist");
            }

            var passwordHash = hashPassword(i_Password);

            var newUser = new UserDTO
            {
                Username = i_Username,
                PasswordHash = passwordHash,
                FullName = i_FullName
            };

            await r_UserRepository.AddUserAsync(newUser);

            var user = await r_UserRepository.GetUserByUsernameAsync(i_Username);

            return user;
        }

        /// <summary>
        /// Hashes the password using SHA-256.
        /// </summary>
        /// <param name="i_Password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        private string hashPassword(string i_Password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(i_Password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Authenticates a user by username and password asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user to authenticate.</param>
        /// <param name="i_Password">The password of the user to authenticate.</param>
        /// <returns>The authenticated User entity.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        public async Task<User> AuthenticateUserByUsernameAsync(string i_Username, string i_Password)
        {
            var user = await r_UserRepository.GetUserByUsernameAsync(i_Username);

            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            if (user.PasswordHash != i_Password && !user.VerifyPassword(i_Password))
            {
                throw new AuthenticateException("Invalid password");
            }

            user.Conversations.Sort();
            user.Conversations.Reverse();

            return user;
        }

        /// <summary>
        /// Retrieves all user contacts asynchronously.
        /// </summary>
        /// <returns>A list of UserContact entities.</returns>
        public async Task<IEnumerable<UserContact>> GetAllUserContactsAsync()
        {
            return await r_UserRepository.GetAllUserContactAsync();
        }

        /// <summary>
        /// Retrieves a user contact by username asynchronously.
        /// </summary>
        /// <param name="i_Username">The username of the user contact to retrieve.</param>
        /// <returns>The UserContact entity.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        public async Task<UserContact> GetUserContactByUsernameAsync(string i_Username)
        {
            try
            {
                var contact = await r_UserRepository.GetUserContactByUsernameAsync(i_Username);

                if (contact == null)
                {
                    throw new UserNotFoundException();
                }

                return contact;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a user contact by ID asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the user contact to retrieve.</param>
        /// <returns>The UserContact entity.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        public async Task<UserContact> GetUserContactByIdAsync(int i_Id)
        {
            try
            {
                var dto = await r_UserRepository.GetUserDTOByIdAsync(i_Id);

                if (dto == null)
                {
                    throw new UserNotFoundException(i_Id);
                }

                return dto.ToUserContact();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sends a contact request asynchronously.
        /// </summary>
        /// <param name="i_RequesterId">The ID of the requester.</param>
        /// <param name="i_Password">The password of the requester.</param>
        /// <param name="i_RecipientId">The ID of the recipient.</param>
        /// <returns>The created ContactRequest entity.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the requester is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        public async Task<ContactRequest> SendContactRequestAsync(int i_RequesterId, string i_Password, int i_RecipientId)
        {
            try
            {
                var user = await r_UserRepository.GetUserDTOByIdAsync(i_RequesterId);

                if (user == null)
                {
                    throw new UserNotFoundException(i_RequesterId);
                }

                if (!user.VerifyPassword(i_Password))
                {
                    throw new AuthenticateException();
                }

                return await r_ContactRequestRepository.CreateContactRequestAsync(i_RequesterId, i_RecipientId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Accepts a contact request asynchronously.
        /// </summary>
        /// <param name="i_RequestId">The ID of the contact request to accept.</param>
        /// <param name="i_Password">The password of the recipient.</param>
        /// <exception cref="EntityNotFoundException">Thrown if the request or recipient is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        public async Task AcceptContactRequestAsync(int i_RequestId, string i_Password)
        {
            try
            {
                var request = await r_ContactRequestRepository.GetContactRequestByIdAsync(i_RequestId);

                if (request == null)
                {
                    throw new EntityNotFoundException("Request not found");
                }

                var userRecipient = await r_UserRepository.GetUserDTOByIdAsync(request.RecipientId);

                if (!userRecipient.VerifyPassword(i_Password))
                {
                    throw new AuthenticateException();
                }

                await r_ContactRequestRepository.AcceptContactRequestAsync(request);
                await r_ContactRequestRepository.DeleteContactRequestAsync(request);

                await r_ConverstionRepository.CreateConverstionAsync(new List<int> { request.RecipientId, request.RequesterId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes a contact request asynchronously.
        /// </summary>
        /// <param name="i_RequestId">The ID of the contact request to delete.</param>
        /// <param name="i_Password">The password of the recipient or requester.</param>
        /// <exception cref="EntityNotFoundException">Thrown if the request, requester, or recipient is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        public async Task DeleteContactRequestAsync(int i_RequestId, string i_Password)
        {
            try
            {
                var request = await r_ContactRequestRepository.GetContactRequestByIdAsync(i_RequestId);

                if (request == null)
                {
                    throw new EntityNotFoundException("Request not found");
                }

                var userRecipient = await r_UserRepository.GetUserDTOByIdAsync(request.RecipientId);
                var userRequester = await r_UserRepository.GetUserDTOByIdAsync(request.RequesterId);

                if (userRequester == null || userRecipient == null)
                {
                    throw new EntityNotFoundException("Requester or Recipient not found");
                }

                if (!userRequester.VerifyPassword(i_Password) || !userRecipient.VerifyPassword(i_Password))
                {
                    throw new AuthenticateException();
                }

                await r_ContactRequestRepository.DeleteContactRequestAsync(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a contact request by its ID asynchronously.
        /// </summary>
        /// <param name="i_RequestId">The ID of the contact request to retrieve.</param>
        /// <returns>The ContactRequest entity.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the request is not found.</exception>
        public async Task<ContactRequest> GetContactRequestById(int i_RequestId)
        {
            try
            {
                var request = await r_ContactRequestRepository.GetContactRequestByIdAsync(i_RequestId);

                if (request == null)
                {
                    throw new EntityNotFoundException("Request not found");
                }

                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves all contact requests from a specified user asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the requester.</param>
        /// <returns>A list of ContactRequest entities sent by the specified user.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        public async Task<IEnumerable<ContactRequest>> GetAllContactRequestsFromIdAsync(int i_Id)
        {
            try
            {
                if (!(await r_UserRepository.IsUserIdExistAsync(i_Id)))
                {
                    throw new UserNotFoundException(i_Id);
                }

                return await r_ContactRequestRepository.GetAllContactRequestsFromIdAsync(i_Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves all contact requests to a specified user asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the recipient.</param>
        /// <returns>A list of ContactRequest entities received by the specified user.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        public async Task<IEnumerable<ContactRequest>> GetAllContactRequestsToIdAsync(int i_Id)
        {
            try
            {
                if (!(await r_UserRepository.IsUserIdExistAsync(i_Id)))
                {
                    throw new UserNotFoundException(i_Id);
                }

                return await r_ContactRequestRepository.GetAllContactRequestsToIdAsync(i_Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Retrieves all user contacts whose usernames start with the specified prefix asynchronously.
        /// </summary>
        /// <param name="i_StartWith">The prefix to filter usernames by.</param>
        /// <returns>A list of UserContact entities whose usernames start with the specified prefix.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if no users start with the specified username.</exception>
        public async Task<IEnumerable<UserContact>> GetAllUserContectsStartWith(string i_StartWith)
        {
            var userContacts = await r_UserRepository.GetUserContactsWithUsernameStartWithAsync(i_StartWith);

            if (!userContacts.Any())
            {
                throw new ArgumentOutOfRangeException("No users start with this username");
            }

            return userContacts;
        }

        /// <summary>
        /// Deletes a user by ID asynchronously.
        /// </summary>
        /// <param name="i_UserId">The ID of the user to delete.</param>
        /// <param name="i_Password">The password of the user to delete.</param>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        public async Task DeleteUserByIdAsync(int i_UserId, string i_Password)
        {
            try
            {
                var user = await r_UserRepository.GetUserDTOByIdAsync(i_UserId);

                if (user == null)
                {
                    throw new UserNotFoundException(i_UserId);
                }
                else if (user.VerifyPassword(i_Password))
                {
                    throw new AuthenticateException();
                }

                await r_UserRepository.DeleteUserByIdAsync(i_UserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Removes a contact from a user's contact list asynchronously.
        /// </summary>
        /// <param name="i_UserId">The ID of the user.</param>
        /// <param name="i_Password">The password of the user.</param>
        /// <param name="i_ContactId">The ID of the contact to remove.</param>
        /// <exception cref="UserNotFoundException">Thrown if the user is not found.</exception>
        /// <exception cref="AuthenticateException">Thrown if the password is invalid.</exception>
        public async Task RemoveContactFromUser(int i_UserId, string i_Password, int i_ContactId)
        {
            try
            {
                var user = await r_UserRepository.GetUserDTOByIdAsync(i_UserId);

                if (user == null)
                {
                    throw new UserNotFoundException(i_UserId);
                }

                if (!user.VerifyPassword(i_Password))
                {
                    throw new AuthenticateException();
                }

                await r_UserRepository.RemoveContactAsync(user, i_ContactId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
