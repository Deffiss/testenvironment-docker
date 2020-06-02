using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class PizzaContext : DbContext
    {
        private readonly string _connectionString;

        public PizzaContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Pizza> Pizzas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
