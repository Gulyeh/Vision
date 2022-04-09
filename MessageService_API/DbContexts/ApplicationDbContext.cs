using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService_API.Entites;
using Microsoft.EntityFrameworkCore;

namespace MessageService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Messages = Set<Message>();
            MessagesAttachments = Set<MessageAttachment>();
            Chats = Set<Chat>();
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAttachment> MessagesAttachments { get; set; }
        public DbSet<Chat> Chats { get; set; }
    }
}