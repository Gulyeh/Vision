using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Dtos;
using CodesService_API.Entites;

namespace CodesService_API.Repository.IRepository
{
    public interface ICodesRepository
    {
        Task<ResponseDto> CheckCode(string code);
        Task<ResponseDto> GetAllCodes();
        Task<ResponseDto> RemoveCode(string code);
        Task<ResponseDto> EditCode(CodesDataDto codeData);
    }
}