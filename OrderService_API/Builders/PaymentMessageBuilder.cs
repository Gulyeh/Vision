using OrderService_API.Messages;

namespace OrderService_API.Builders
{
    public class PaymentMessageBuilder
    {
        PaymentMessage payment;

        public PaymentMessageBuilder(decimal Price, decimal Discount, CouponDataDto couponData)
        {
            payment = new PaymentMessage(Price, Discount, couponData);
        }

        public void SetUserId(Guid userId)
        {
            payment.UserId = userId;
        }

        public void SetOrderId(Guid orderId)
        {
            payment.OrderId = orderId;
        }

        public void SetPaymentMethodId(Guid paymentMethodId)
        {
            payment.PaymentMethodId = paymentMethodId;
        }

        public void SetTitle(string Title)
        {
            payment.Title = Title;
        }

        public void SetEmail(string Email)
        {
            payment.Email = Email;
        }

        public PaymentMessage Build()
        {
            return payment;
        }
    }
}