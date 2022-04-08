using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Helpers;

namespace GameAccessService_API.Entites
{
    public class UserGames : BaseUser
    {
        [Key]
        public int Id { get; set; }
    }
}