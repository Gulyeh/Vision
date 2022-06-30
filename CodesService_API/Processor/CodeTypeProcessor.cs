using CodesService_API.Helpers;
using CodesService_API.Processor.Interfaces;
using CodesService_API.RabbitMQRPC;
using CodesService_API.RabbitMQSender;

namespace CodesService_API.Processor
{
    public interface ICodeTypeProcessor
    {
        IAccessable? CreateAccessCode(CodeTypes codeType);
        ISender? CreateSenderCode(CodeTypes codeType);
    }

    public class CodeTypeProcessor : ICodeTypeProcessor
    {
        private readonly IRabbitMQSender rabbitMQSender;
        private ProductType? productType { get; set; } = null;
        private readonly IRabbitMQRPC rabbitMQRPC;

        public CodeTypeProcessor(IRabbitMQRPC rabbitMQRPC, IRabbitMQSender rabbitMQSender)
        {
            this.rabbitMQRPC = rabbitMQRPC;
            this.rabbitMQSender = rabbitMQSender;
        }

        public IAccessable? CreateAccessCode(CodeTypes codeType)
        {
            return codeType switch
            {
                CodeTypes.Product => productType != null ? productType : productType = new ProductType(rabbitMQSender, rabbitMQRPC),
                _ => null
            };
        }

        public ISender? CreateSenderCode(CodeTypes codeType)
        {
            return codeType switch
            {
                CodeTypes.Product => productType != null ? productType : productType = new ProductType(rabbitMQSender, rabbitMQRPC),
                CodeTypes.Currency => new CurrencyType(rabbitMQSender),
                _ => null
            };
        }

    }
}