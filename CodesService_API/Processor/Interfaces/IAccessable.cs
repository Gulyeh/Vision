namespace CodesService_API.Processor.Interfaces
{
    public interface IAccessable
    {
        Task<bool> CheckAccess(Guid? gameId, string productId, Guid userId);
    }
}