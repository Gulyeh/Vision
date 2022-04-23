using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Helpers;
using CodesService_API.Proccessor.Interfaces;
using CodesService_API.RabbitMQSender;
using CodesService_API.Services.IServices;

namespace CodesService_API.Proccessor
{
    public class ProductType : IAccessableSender
    {
        public IRabbitMQSender rabbitMQSender { get; set; }
        public IGameAccessService gameAccessService { get; set; }

        public ProductType(IRabbitMQSender rabbitMQSender, IGameAccessService gameAccessService)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.gameAccessService = gameAccessService;
        }

        public async Task<bool> CheckAccess(string? gameId, string Access_Token, string? productId = null)
        {
            if(await gameAccessService.CheckAccess(gameId, Access_Token, productId)) return true;
            return false;
        }

        public void SendRabbitMQMessage(Guid userId, Guid? gameId = null, string? productId= null)
        {
            rabbitMQSender.SendMessage(new { userId = userId, gameId = gameId, productId = productId }, "ProductCuponUsedQueue");
        }

        public ResponseDto GetResponse(Codes data)
        {
            var responseData = new ResponseCode();
            responseData.GameId = data.gameId;
            responseData.ProductId = data.CodeValue;
            responseData.CodeType = CodeTypes.Product;
            responseData.Title = data.Title;

            return new ResponseDto(true, StatusCodes.Status200OK, responseData);
        }
    }
}