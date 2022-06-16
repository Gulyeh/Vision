using CodesService_API.Helpers;
using CodesService_API.Processor.Interfaces;
using CodesService_API.RabbitMQSender;
using CodesService_API.Services.IServices;

namespace CodesService_API.Processor
{
    public interface ICodeTypeProcessor
    {
        IAccessable? CreateAccessCode(CodeTypes codeType);
        ISender? CreateSenderCode(CodeTypes codeType);
    }

    public class CodeTypeProcessor : ICodeTypeProcessor
    {
        private readonly IGameAccessService gameAccessService;
        private readonly IRabbitMQSender rabbitMQSender;
        private ProductType? productType { get; set; } = null;

        public CodeTypeProcessor(IGameAccessService gameAccessService, IRabbitMQSender rabbitMQSender)
        {
            this.gameAccessService = gameAccessService;
            this.rabbitMQSender = rabbitMQSender;
        }

        public IAccessable? CreateAccessCode(CodeTypes codeType)
        {
            return codeType switch
            {
                CodeTypes.Product => productType != null ? productType : productType = new ProductType(rabbitMQSender, gameAccessService),
                _ => null
            };
        }

        public ISender? CreateSenderCode(CodeTypes codeType)
        {
            return codeType switch
            {
                CodeTypes.Product => productType != null ? productType : productType = new ProductType(rabbitMQSender, gameAccessService),
                CodeTypes.Currency => new CurrencyType(rabbitMQSender),
                _ => null
            };
        }

    }
}