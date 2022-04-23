using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Proccessor.Interfaces;
using CodesService_API.Helpers;
using CodesService_API.RabbitMQSender;
using CodesService_API.Services.IServices;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Messages;

namespace CodesService_API.Proccessor
{
    public class CurrencyType : ISender
    {
        public IRabbitMQSender rabbitMQSender { get; set; }

        public CurrencyType(IRabbitMQSender rabbitMQSender)
        {
            this.rabbitMQSender = rabbitMQSender;
        }

        public void SendRabbitMQMessage(Guid userId, Guid? gameId = null, string? productId = null)
        {
            int amount = 0;
            int.TryParse(productId, out amount);
            rabbitMQSender.SendMessage(new CurrencyDto { UserId = userId, Amount = amount, isCode = true }, "ChangeFundsQueue");
        }

        public ResponseDto GetResponse(Codes data)
        {
            var responseData = new ResponseCode();
            responseData.ProductId = data.CodeValue;
            responseData.CodeType = CodeTypes.Currency;
            responseData.Title = $"{data.CodeValue} Visions";

            return new ResponseDto(true, StatusCodes.Status200OK, responseData);
        }
    }
}