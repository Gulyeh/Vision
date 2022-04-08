using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UsersService_API.Helpers;

namespace UsersService_API.Dtos
{
    public class UserDataDto : BaseUserData
    {
        public string? Description { get; set; }
    }
}