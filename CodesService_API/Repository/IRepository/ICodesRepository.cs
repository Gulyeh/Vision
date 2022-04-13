using CodesService_API.Dtos;

namespace CodesService_API.Repository.IRepository
{
    public interface ICodesRepository
    {
        Task<ResponseDto> CheckCode(string code);
        Task<ResponseDto> AddCode(AddCodesDto code);
        Task<ResponseDto> GetAllCodes();
        Task<ResponseDto> RemoveCode(int codeId);
        Task<ResponseDto> EditCode(CodesDataDto codeData);
    }
}