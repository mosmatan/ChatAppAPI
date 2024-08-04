using ChatAPI.Models;

namespace ChatAPI.Repositories
{
    public interface IContactRequestRepository
    {
        /// <summary>
        /// Creates a new contact request asynchronously.
        /// </summary>
        /// <param name="i_RequesterId">The ID of the requester.</param>
        /// <param name="i_RecipientId">The ID of the recipient.</param>
        /// <returns>The created ContactRequest.</returns>
        /// <exception cref="ArgumentException">Thrown if the request already exists or the users are already connected.</exception>
        Task<ContactRequest> CreateContactRequestAsync(int i_RequesterId, int i_RecipientId);

        /// <summary>
        /// Retrieves all contact requests to a specified user asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the recipient.</param>
        /// <returns>A list of ContactRequest entities received by the specified user.</returns>
        Task<IEnumerable<ContactRequest>> GetAllContactRequestsToIdAsync(int i_Id);

        /// <summary>
        /// Retrieves all contact requests from a specified user asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the requester.</param>
        /// <returns>A list of ContactRequest entities sent by the specified user.</returns>
        Task<IEnumerable<ContactRequest>> GetAllContactRequestsFromIdAsync(int i_Id);

        /// <summary>
        /// Accepts a contact request by adding each user to the other's contact list asynchronously.
        /// </summary>
        /// <param name="i_ContactRequest">The contact request to accept.</param>
        /// <exception cref="UserNotFoundException">Thrown if either user is not found.</exception>
        Task AcceptContactRequestAsync(ContactRequest i_ContactRequest);

        /// <summary>
        /// Deletes a contact request asynchronously.
        /// </summary>
        /// <param name="i_ContactRequest">The contact request to delete.</param>
        Task DeleteContactRequestAsync(ContactRequest i_ContactRequest);

        /// <summary>
        /// Checks if a contact request exists by its ID asynchronously.
        /// </summary>
        /// <param name="i_RequestId">The ID of the contact request.</param>
        /// <returns>True if the contact request exists, false otherwise.</returns>
        Task<bool> IsRequestIdExistAsync(int i_RequestId);

        /// <summary>
        /// Retrieves a contact request by its ID asynchronously.
        /// </summary>
        /// <param name="i_Id">The ID of the contact request.</param>
        /// <returns>A ContactRequest entity if found, null otherwise.</returns>
        Task<ContactRequest?> GetContactRequestByIdAsync(int i_Id);
    }
}
