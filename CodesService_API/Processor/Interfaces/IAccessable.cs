namespace CodesService_API.Processor.Interfaces
{
    public interface IAccessable
    {
        Task<bool> CheckAccess(Guid? gameId, string Access_Token, string productId);
    }
}