using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Repository;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services;
using VisionClient.Core.Services.IServices;
using VisionClient.Utility;

namespace VisionClient.Extensions
{
    internal static class RegisterLoginDependenciesExtension
    {
        internal static void RegisterLoginDependencies(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterServices(s =>
            {
                s.AddHttpClient<IAccountService, AccountService>();
                s.AddScoped<IAccountService, AccountService>();
                s.AddScoped<IAccountRepository, AccountRepository>();
                s.AddScoped<IXMLCredentials, XMLCredentials>();
            });
        }
    }
}
