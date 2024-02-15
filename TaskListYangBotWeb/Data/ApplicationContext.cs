using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FavoriteTask> FavoriteTasks { get; set; }
        public DbSet<UserWeb> UsersWeb { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<FavoriteEnvironment> FavoriteEnvironments { get; set; }
    }
}
