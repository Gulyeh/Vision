using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using HashidsNet;

namespace CodesService_API.Extensions
{
    public static class ListExtensions
    {
        public static List<CodesDataDto> ListIdsHasher (this List<CodesDataDto> codesList, IHashids hashids){
            foreach(var code in codesList){
                code.Id = hashids.Encode(int.Parse(code.Id));
            }
            return codesList;
        }
    }
}