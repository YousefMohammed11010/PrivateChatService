using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PrivateChatService.Models;

namespace PrivateChatService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();
    }
}