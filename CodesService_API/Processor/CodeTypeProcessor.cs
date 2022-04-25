using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Helpers;
using CodesService_API.Processor;
using CodesService_API.Processor.Interfaces;
using CodesService_API.RabbitMQSender;
using CodesService_API.Services.IServices;

namespace CodesService_API.Processor
{
    public class CodeTypeProcessor
    {
        private readonly IGameAccessService gameAccessService;
        private readonly IRabbitMQSender rabbitMQSender;
        private GameType? gameType { get; set; } = null;
        private ProductType? productType { get; set; } = null;

        public CodeTypeProcessor(IGameAccessService gameAccessService, IRabbitMQSender rabbitMQSender)
        {
            this.gameAccessService = gameAccessService;
            this.rabbitMQSender = rabbitMQSender;
        }

        public IAccessable? CreateAccessCode(CodeTypes codeType)
        {
            return codeType switch{
                CodeTypes.Game => gameType != null ? gameType : gameType = new GameType(rabbitMQSender, gameAccessService),
                CodeTypes.Product => productType != null ? productType : productType = new ProductType(rabbitMQSender, gameAccessService),
                _ => null
            };
        }

        public ISender? CreateSenderCode(CodeTypes codeType)
        {
            return codeType switch{
                CodeTypes.Game => gameType != null ? gameType : gameType = new GameType(rabbitMQSender, gameAccessService),
                CodeTypes.Product => productType != null ? productType : productType = new ProductType(rabbitMQSender, gameAccessService),
                CodeTypes.Currency => new CurrencyType(rabbitMQSender),
                _ => null
            };
        }

    }
}