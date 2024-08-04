using ChatAPI.Data;
using ChatAPI.Models;
using ChatAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Repositories
{
    public class ContactRequestRepository : IContactRequestRepository
    {
        private readonly ChatDbContext r_Context;

        public ContactRequestRepository(ChatDbContext i_Context)
        {
            r_Context = i_Context;
        }

        /// <summary>
        /// Accepts a contact request by adding each user to the other's contact list asynchronously.
        /// </summary>
        /// <param name="i_ContactRequest">The contact request to accept.</param>
        /// <exception cref="UserNotFoundException">Thrown if either user is not found.</exception>
        public async Task AcceptContactRequestAsync(ContactRequest i_ContactRequest)
        {
            var user1 = await r_Context.Users.FindAsync(i_ContactRequest.RecipientId);
            var user2 = await r_Context.Users.FindAsync(i_ContactRequest.RequesterId);

            if (user1 == null)
            {
                throw new UserNotFoundException(i_ContactRequest.RecipientId);
            }
            else if (user2 == null)
            {
                throw new UserNotFoundException(i_ContactRequest.RequesterId);
            }

            user1.AddContactId(user2.UserId);
            user2.AddContactId(user1.UserId);

            r_Context.Update(user1);
            r_Context.Update(user2);
            r_Context.Update(i_ContactRequest);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a new contact request asynchronously.
        /// </summary>
        /// <param name="i_RequesterId">The ID of the requester.</param>
        /// <param name="i_RecipientId">The ID of the recipient.</param>
        /// <returns>The created ContactRequest.</returns>
        /// <exception cref="ArgumentException">Thrown if the request already exists or the users are already connected.</exception>
        public async Task<ContactRequest> CreateContactRequestAsync(int i_RequesterId, int i_RecipientId)
        {
            bool isValidRequest = !(await isUsersConnectAsync(i_RequesterId, i_RecipientId) || await isRequestExistByUsersIdsAsync(i_RequesterId, i_RecipientId));

            if (!isValidRequest)
            {
                throw new ArgumentException("Request already exist or Users already connect");
            }

            var contactRequest = new ContactRequest(i_RequesterId, i_RecipientId);

            await r_Context.AddAsync(contactRequest);
            await r_Context.SaveChangesAsync();

            return contactRequest;
        }

        /// <summary>
        /// Deletes a contact request asynchronously.
        /// </summary>
        /// <param name="i_ContactRequest">The contact request to delete.</param>
        public async Task DeleteContactRequestAsync(ContactRequest i_ContactRequest)
        {
            r_Context.Remove(i_ContactRequest);
            await r_Context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all contact requests from a specified user asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the requester.</param>
        /// <returns>A list of ContactRequest entities sent by the specified user.</returns>
        public async Task<IEnumerable<ContactRequest>> GetAllContactRequestsFromIdAsync(int i_Id)
        {
            var requests = await r_Context.ContactRequests.Where(request => request.RequesterId == i_Id).ToListAsync();
            return requests;
        }

        /// <summary>
        /// Retrieves all contact requests to a specified user asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the recipient.</param>
        /// <returns>A list of ContactRequest entities received by the specified user.</returns>
        public async Task<IEnumerable<ContactRequest>> GetAllContactRequestsToIdAsync(int i_Id)
        {
            var requests = await r_Context.ContactRequests.Where(request => request.RecipientId == i_Id).ToListAsync();
            return requests;
        }

        /// <summary>
        /// Retrieves a contact request by its ID asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the contact request.</param>
        /// <returns>A ContactRequest entity if found, null otherwise.</returns>
        public async Task<ContactRequest?> GetContactRequestByIdAsync(int i_Id)
        {
            return await r_Context.ContactRequests.FindAsync(i_Id);
        }

        /// <summary>
        /// Checks if a contact request exists by its ID asynchronously.
        /// </summary>
        /// <param name="i_RequestId">The ID of the contact request.</param>
        /// <returns>True if the contact request exists, false otherwise.</returns>
        public async Task<bool> IsRequestIdExistAsync(int i_RequestId)
        {
            return await r_Context.ContactRequests.FindAsync(i_RequestId) != null;
        }

        /// <summary>
        /// Checks if a contact request exists between two users asynchronously.
        /// </summary>
        /// <param name="i_RequesterId">The ID of the requester.</param>
        /// <param name="i_RecipientId">The ID of the recipient.</param>
        /// <returns>True if a contact request exists between the users, false otherwise.</returns>
        private async Task<bool> isRequestExistByUsersIdsAsync(int i_RequesterId, int i_RecipientId)
        {
            var requests = await GetAllContactRequestsFromIdAsync(i_RecipientId);

            foreach (var request in requests)
            {
                int[] ids = { request.RecipientId, request.RequesterId };

                if (ids.Contains(i_RequesterId) && ids.Contains(i_RecipientId))
                {
                    return true;
                }
            }

            requests = await GetAllContactRequestsFromIdAsync(i_RequesterId);

            foreach (var request in requests)
            {
                int[] ids = { request.RecipientId, request.RequesterId };

                if (ids.Contains(i_RequesterId) && ids.Contains(i_RecipientId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if two users are connected asynchronously.
        /// </summary>
        /// <param name="i_UserId1">The ID of the first user.</param>
        /// <param name="i_UserId2">The ID of the second user.</param>
        /// <returns>True if the users are connected, false otherwise.</returns>
        /// <exception cref="Exception">Thrown if the first user is not found.</exception>
        private async Task<bool> isUsersConnectAsync(int i_UserId1, int i_UserId2)
        {
            var user = await r_Context.Users.FindAsync(i_UserId1);

            if (user == null)
            {
                throw new Exception($"User with id {i_UserId1} not found");
            }

            return user.ContactIds.Contains(i_UserId2);
        }
    }
}
