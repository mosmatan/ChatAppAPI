using ChatAPI.Exceptions;
using ChatAPI.Models.Requests;
using ChatAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController :ControllerBase
    {
        private readonly UserService r_UserService;

        public UserController(UserService userService)
        {
            r_UserService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var user = await r_UserService.RegisterUserAsync(request.Username, request.Password, request.FullName);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string  username, [FromQuery] string password)
        {
            try
            {
                var user = await r_UserService.AuthenticateUserByUsernameAsync(username, password);

                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AuthenticateException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("Delete/{i_Id}")]
        public async Task<IActionResult> DeleteUser(int i_Id, [FromQuery] string password)
        {
            try
            {
                await r_UserService.DeleteUserByIdAsync(i_Id, password);

                return Ok();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(AuthenticateException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                var contacts = await r_UserService.GetAllUserContactsAsync();
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("contacts/remove")]
        public async Task<IActionResult> RemoveContactAsync([FromQuery] int userId, [FromQuery] string password, [FromQuery] int contactId)
        {
            try
            {
                await r_UserService.RemoveContactFromUser(userId, password, contactId);

                return Ok();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AuthenticateException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contact/username/{i_Username}")]
        public async Task<IActionResult> GetContactByUsername(string i_Username)
        {
            try
            {
                var userContact = await r_UserService.GetUserContactByUsernameAsync(i_Username);

                return Ok(userContact);
            }
            catch(UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contact/id/{i_Id}")]
        public async Task<IActionResult> GetContactById(string i_Id)
        {
            bool isConvertSuccess = int.TryParse(i_Id, out var contactId);

            if(!isConvertSuccess)
            {
                return BadRequest("Invalid user id");
            }

            try
            {
                var userContact = await r_UserService.GetUserContactByIdAsync(contactId);

                return Ok(userContact);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("contact/sendcontactrequest")]
        public async Task<IActionResult> AddContactToUser([FromBody] AddContactRequest i_Request)
        {
            try
            {
                var contactRequest = await r_UserService.SendContactRequestAsync(i_Request.RequesterId, i_Request.RequesterPassword, i_Request.RecipientId);

                return Ok(new { Message = "Requst has sent", Requst = contactRequest });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AuthenticateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("contact/responsecontactrequest")]
        public async Task<IActionResult> ResponseContactRequest([FromBody] ContactRequestAnswer i_Answer)
        {
            try
            {
                if (i_Answer.IsAprroved)
                {
                    await r_UserService.AcceptContactRequestAsync(i_Answer.RequestId, i_Answer.Password);
                }
                else
                {
                    await r_UserService.DeleteContactRequestAsync(i_Answer.RequestId, i_Answer.Password);
                }

                return Ok("Request has handled");
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AuthenticateException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contact/requests/{i_Id}")]
        public async Task<IActionResult> GetContactRequestById(int i_Id)
        {
            try
            {
                var request = await r_UserService.GetContactRequestById(i_Id);

                return Ok(request);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contact/requestsentfrom/{i_Id}")]
        public async Task<IActionResult> GetContactRequestFromUser(int i_Id)
        {
            try
            {
                var contactRequests = await r_UserService.GetAllContactRequestsFromIdAsync(i_Id);

                return Ok(contactRequests);
            }
            catch(UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contact/requestsentto/{i_Id}")]
        public async Task<IActionResult> GetContactRequestToUser(int i_Id)
        {
            try
            {
                var contactRequests = await r_UserService.GetAllContactRequestsToIdAsync(i_Id);

                return Ok(contactRequests);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("contact/startwith/{i_StartWith}")]
        public async Task<IActionResult> GetAllUserContactsStartWith(string i_StartWith)
        {
            try
            {
                var contacts = await r_UserService.GetAllUserContectsStartWith(i_StartWith);

                return Ok(contacts);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }
    }
}
