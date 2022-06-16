using MessageService_API.Dtos;
using MessageService_API.Repository.IRepository;
using MessagesService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessageService_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageRepository messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        [HttpGet("FriendsUnreadMessages")]
        public async Task<ActionResult<ResponseDto>> FriendsUnreadMessages([FromBody] ICollection<Guid> FriendsList)
        {
            var UserId = HttpContext.User.GetId();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, await messageRepository.CheckUnreadMessages(FriendsList, UserId)));
        }
    }
}