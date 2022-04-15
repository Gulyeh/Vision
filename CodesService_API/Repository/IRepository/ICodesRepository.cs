using CodesService_API.Dtos;
using CodesService_API.Helpers;

namespace CodesService_API.Repository.IRepository
{
    public interface ICodesRepository
    {
        Task<ResponseDto> CheckCode(string code, CodeTypes codeType);
        Task<ResponseDto> AddCode(AddCodesDto code);
        Task<ResponseDto> GetAllCodes();
        Task<ResponseDto> RemoveCode(int codeId);
        Task<ResponseDto> EditCode(CodesDataDto codeData);
        Task<ResponseDto> ApplyCode(string code, Guid userId, CodeTypes codeType, string Access_Token);
    }
}