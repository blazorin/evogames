using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Model.Data
{
    public class EvoGamesContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Filename=EvoContext.db",
                opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            base.OnConfiguring(builder);
        }
    }
}