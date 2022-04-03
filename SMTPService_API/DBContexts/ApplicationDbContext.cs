using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMTPService_API.Entites;

namespace SMTPService_API.DBContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            EmailLogs = Set<EmailLogs>();
        }

        public DbSet<EmailLogs> EmailLogs { get; set; }
    }
}