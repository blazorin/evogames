using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Model.Data
{
    public class EvoGamesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPerms> UserPerms { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Filename=EvoContext.db",
                opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<User>().HasIndex(u => u.Email).IsUnique(true);
            model.Entity<Bet>().HasIndex(b => b.Date).IsUnique(false);
            model.Entity<Transaction>().HasIndex(t => t.Date).IsUnique(false);
            model.Entity<UserLog>().HasIndex(l => l.Date).IsUnique(false);
        }
    }
}