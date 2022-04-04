using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Helpers;

namespace CodesService_API.Entites
{
    public class Codes : BasicCodes
    {
        [Key]
        public int Id { get; set; }
    }
}