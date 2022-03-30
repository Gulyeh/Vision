using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SMTPService_API.DBContexts;
using SMTPService_API.Repository;
using SMTPService_API.Repository.IRepository;
using SMTPService_API.Services;
using SMTPService_API.Services.IServices;

namespace SMTPService_API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection service, IConfiguration config){
            service.AddDbContext<ApplicationDbContext>(opts => {
                opts.UseSqlServer(config.GetConnectionString("Connection"));
            });

            service.AddScoped<IEmailRepository, EmailRepository>();
            service.AddScoped<IEmailService, EmailService>();
            return service;
        }
    }
}