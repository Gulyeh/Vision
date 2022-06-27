using CodesService_API.Dtos;
using CodesService_API.Helpers;

namespace CodesService_API.Repository.IRepository
{
    public interface ICodesRepository
    {
        Task<ResponseDto> CheckCode(string code, CodeTypes codeType, Guid userId);
        Task<ResponseDto> AddCode(AddCodesDto code);
        Task<ResponseDto> GetAllCodes();
        Task<ResponseDto> RemoveCode(string code);
        Task<ResponseDto> EditCode(EditCodeDto codeData);
        Task<ResponseDto> ApplyCode(string code, Guid userId, CodeTypes codeType, string Access_Token);
        Task RemoveUsedCode(string code, Guid userId);
        Task<ResponseDto> GetUserUsedCodes(Guid userId);
        Task<ResponseDto> RemoveUsedCode(Guid codeId);
    }
}