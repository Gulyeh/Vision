using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UsersService_API.Dtos;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;

namespace UsersService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ICacheService cacheService;
        private readonly IHubContext<UsersHub> hubContext;

        public UsersController(IUserRepository userRepository, ICacheService cacheService, IHubContext<UsersHub> hubContext)
        {
            this.userRepository = userRepository;
            this.cacheService = cacheService;
            this.hubContext = hubContext;
        }

        [HttpGet("FindUser")]
        public async Task<ActionResult<ResponseDto>> FindUsers([FromQuery]string containsString){
            return new ResponseDto(true, StatusCodes.Status200OK, await userRepository.FindUsers(containsString));
        }

        [HttpGet("MessageNotification")]
        public async Task<ActionResult> SendUserMessageNotification([FromQuery] Guid receiverId, [FromQuery] Guid chatId){
            var response = await userRepository.UserExists(receiverId);
            if(response.Response is true){
                var connIds = await cacheService.TryGetFromCache();
                if(connIds.ContainsKey(receiverId)){
                    var userIds = connIds.GetValueOrDefault(receiverId);
                    if(userIds is not null) 
                    {
                        await hubContext.Clients.Clients(userIds).SendAsync("ChatNotification", chatId);
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }
    }
}