using SMTPService_API.Generator.Interfaces;

namespace SMTPService_API.Generator
{
    public class PaymentContent : IContent
    {
        public string GenerateContent(string content)
        {
            return string.Format($@"<html>
                        <head>
                        <meta content=""text/html; charset=UTF-8"" http-equiv=""Content-Type"" />
                        </head>
                        <tbody>
                            <div style=""background-color: #164971;height: 300px; width: 700px;"" >
                                <div style=""display: webkit-flex;text-align: center; height: 100px; vertical-align: top;"" >
                                    <img src=""https://res.cloudinary.com/dhj8btqwp/image/upload/v1655237540/VisionLogo_aq9pzl.png"" height=""60"" width=""60"" style=""margin-top: 20px;""/>
                                </div>
                                <div style=""vertical-align:bottom;height: 200px;"">
                                    <h2 style=""color: White;display: webkit-flex;text-align: center;margin: 0"">
                                        Thank you for your purchase
                                    </h2>
                                    <h5 style=""color: White;display: webkit-flex;text-align: center;margin-top: 20px;"">Your order has been completed!</h5>
                                    <p style=""margin-top: 40px;color: White;display: webkit-flex;text-align: center;font-size: 15px;"">
                                        You have purchased
                                    </p>
                                    <p style=""margin-top: 10px;color: White;display: webkit-flex;text-align: center;font-size: 25px;font-weight: bold;"">
                                        {content}
                                    </p>
                                </div>
                            </div>
                        </tbody>
                    </html>");
        }

        public string GenerateSubject() => "Payment Confirmation";
    }
}