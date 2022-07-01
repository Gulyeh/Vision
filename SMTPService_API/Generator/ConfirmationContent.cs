using SMTPService_API.Generator.Interfaces;

namespace SMTPService_API.Generator
{
    public class ConfirmationContent : IContent
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
                                        Confirm email
                                    </h2>
                                    <h5 style=""color: White;display: webkit-flex;text-align: center;margin-top: 20px;"">Please confirm your email in order to continue</h5>
                                    <div style=""display: webkit-flex;text-align: center;"">
                                        <a href=""{content}"">
                                            <button style=""margin-top: 20px;color: White;font-size: 20px; height: 40px;width: 160px;background-color: #0AA1DD;border-color: transparent;"">
                                                Confirm
                                            </button>
                                        </a>
                                    </div>
                                </div>
                            </div>
                            </tbody>
                        </html>");
        }

        public string GenerateSubject() => "Account Confirmation";
    }
}