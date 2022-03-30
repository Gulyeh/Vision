using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Entites;

namespace Identity_API.Dtos
{
    public class LoginDto : BasicUserData
    {
        public LoginDto(string password, string email) : base(password, email)
        {
        }
    }
}