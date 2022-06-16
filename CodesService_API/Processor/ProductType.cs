using CodesService_API.Messages;
using CodesService_API.Processor.Interfaces;
using CodesService_API.RabbitMQSender;
using CodesService_API.Services.IServices;

namespace CodesService_API.Processor
{
    public class ProductType : IAccessableSender
    {
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly IGameAccessService gameAccessService;

        public ProductType(IRabbitMQSender rabbitMQSender, IGameAccessService gameAccessService)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.gameAccessService = gameAccessService;
        }

        public async Task<bool> CheckAccess(Guid? gameId, string Access_Token, string productId)
        {
            Guid.TryParse(productId, out Guid ProductId);
            Guid GameId = gameId is null ? Guid.Empty : (Guid)gameId;
            return await gameAccessService.CheckAccess(GameId, Access_Token, ProductId);
        }

        public void SendRabbitMQMessage(Guid userId, Guid? gameId, string codeValue, string code)
        {
            Guid.TryParse(codeValue, out Guid productId);
            Guid GameId = gameId is null ? Guid.Empty : (Guid)gameId;
            rabbitMQSender.SendMessage(new ProductDto(userId, GameId, productId, code), "ProductCuponUsedQueue");
        }
    }
}