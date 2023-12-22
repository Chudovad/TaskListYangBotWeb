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
            builder.Entity<UserFavoriteTask>()
                .HasKey(ur => new { ur.UserId, ur.FavoriteTaskId });
            builder.Entity<UserFavoriteTask>()
                .HasOne(u => u.User)
                .WithMany(us => us.UserFavoriteTasks)
                .HasForeignKey(u => u.UserId);
            builder.Entity<UserFavoriteTask>()
                .HasOne(u => u.FavoriteTask)
                .WithMany(us => us.UserFavoriteTasks)
                .HasForeignKey(u => u.FavoriteTaskId);

            base.OnModelCreating(builder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FavoriteTask> FavoriteTasks { get; set; }
        public DbSet<UserFavoriteTask> UserFavoriteTasks { get; set; }
    }
}
