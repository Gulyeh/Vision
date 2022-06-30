using CodesService_API.Helpers;
using CodesService_API.Messages;
using CodesService_API.Processor.Interfaces;
using CodesService_API.RabbitMQRPC;
using CodesService_API.RabbitMQSender;

namespace CodesService_API.Processor
{
    public class ProductType : IAccessableSender
    {
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly IRabbitMQRPC rabbitMQRPC;

        public ProductType(IRabbitMQSender rabbitMQSender, IRabbitMQRPC rabbitMQRPC)
        {
            this.rabbitMQRPC = rabbitMQRPC;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<bool> CheckAccess(Guid? gameId, string productId, Guid userId)
        {
            Guid.TryParse(productId, out Guid ProductId);
            Guid GameId = gameId is null ? Guid.Empty : (Guid)gameId;
            var type = GameId == Guid.Empty ? OrderType.Game : OrderType.Product;

            var existsResponse = await rabbitMQRPC.SendAsync("CheckProductExistsQueue", new CheckProductExistsDto(ProductId, GameId, type));
            if (existsResponse is null || string.IsNullOrWhiteSpace(existsResponse) || !bool.Parse(existsResponse)) return true;

            var response = await rabbitMQRPC.SendAsync("CheckProductAccessQueue", new CheckProductAccessDto(userId, ProductId, GameId));
            if (response is null || string.IsNullOrWhiteSpace(response)) return true;
            return bool.Parse(response);
        }

        public void SendRabbitMQMessage(Guid userId, Guid? gameId, string codeValue, string code)
        {
            Guid.TryParse(codeValue, out Guid productId);
            Guid GameId = gameId is null ? Guid.Empty : (Guid)gameId;
            rabbitMQSender.SendMessage(new ProductDto(userId, GameId, productId, code), "ProductCuponUsedQueue");
        }
    }
}