using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService_API.Dtos;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet("FindUser")]
        public async Task<ActionResult<ResponseDto>> FindUsers([FromQuery]string containsString){
            return new ResponseDto(true, StatusCodes.Status200OK, await userRepository.FindUsers(containsString));
        }
    }
}