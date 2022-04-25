using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Services.IServices;

namespace Identity_API.Builders
{
    public class UserDtoBuilder
    {
        private UserDto userDto = new();
        private readonly ITokenService tokenService;
        private readonly ApplicationUser user;

        public UserDtoBuilder(ITokenService tokenService, ApplicationUser user)
        {
            this.tokenService = tokenService;
            this.user = user;
        }

        public async Task SetToken(){
            userDto.Token = await tokenService.CreateToken(user);
        }

        public void SetEmail(){
            userDto.Email = user.Email;
        }

        public UserDto Build(){
            return userDto;
        }
    }
}