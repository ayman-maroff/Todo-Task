using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Core.Identity;


namespace TodoApp.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TodoItem> TodoItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Invitation> Invitations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        }
    }
}
