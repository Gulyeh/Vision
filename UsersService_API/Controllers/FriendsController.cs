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
    public class FriendsController : BaseApiController
    {
        private readonly IFriendsRepository friendsRepository;

        public FriendsController(IFriendsRepository friendsRepository)
        {
            this.friendsRepository = friendsRepository;
        }

        [HttpGet("GetFriends")]
        public async Task<ActionResult<ResponseDto>> GetFriends(){
            return CheckActionResult(await friendsRepository.GetFriends(User.GetId()));
        }

        [HttpGet("GetFriendRequests")]
        public async Task<ActionResult<ResponseDto>> GetFriendRequests(){
            return CheckActionResult(await friendsRepository.GetFriendRequests(User.GetId()));
        }

        [HttpGet("GetPendingRequests")]
        public async Task<ActionResult<ResponseDto>> GetPendingRequests(){
            return CheckActionResult(await friendsRepository.GetPendingRequests(User.GetId()));
        }
    }
}