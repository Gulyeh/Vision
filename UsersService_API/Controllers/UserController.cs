using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Extensions;
using Microsoft.AspNetCore.Mvc;
using UsersService_API.Dtos;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet("GetUserData")]
        public async Task<ActionResult<ResponseDto>> GetUserData(){
            return new ResponseDto(true, StatusCodes.Status200OK, await userRepository.GetUserData(User.GetId()));
        }
    }
}