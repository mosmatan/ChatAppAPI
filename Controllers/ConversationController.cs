using ChatAPI.Models.Requests;
using ChatAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using ChatAPI.Models;
using ChatAPI.Exceptions;

namespace ChatAPI.Controllers
{

    [Route("/api/[controller]")]
    [ApiController]
    public class ConversationController: ControllerBase
    {
        private readonly ConversationService r_ConversationService;

        public ConversationController(ConversationService i_MessageService)
        {
            r_ConversationService = i_MessageService;
        }

        [HttpGet("{i_Id}")]
        public async Task<IActionResult> GetConversationById(int i_Id, [FromQuery] int userId, [FromQuery] string password)
        {
            try
            {
                var response = await r_ConversationService.GetConversationAsync(i_Id, userId, password);

                return Ok(response);
            }
            catch (AuthenticateException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest i_Message)
        {
            try
            {
                await r_ConversationService.SendMessageAsync(i_Message.ConversationId, i_Message.SenderId, i_Message.SenderUsername, i_Message.SenderPassword, DateTime.Now, i_Message.Content);

                return Ok();
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


        [HttpGet("getconversations/{i_UserId}")]
        public async Task<IActionResult> GetConvesationsDTOByUserId(int i_UserId, [FromQuery] string password, [FromQuery] List<int> conversationsId)
        {
            try
            {
                var result = await r_ConversationService.GetChoosenUserConverstionsAsync(i_UserId, password, conversationsId);
                return Ok(result);
            }
            catch (AuthenticateException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
