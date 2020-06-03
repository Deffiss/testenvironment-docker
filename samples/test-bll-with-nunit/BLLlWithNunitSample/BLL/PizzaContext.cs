using Microsoft.EntityFrameworkCore;

namespace BLL
{
    public class PizzaContext : DbContext
    {
        private readonly string _connectionString;

        public PizzaContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Pizza> Pizzas { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<PizzaOrder> PizzaOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PizzaOrder>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.PizzaId });
                entity.HasOne<Order>().WithMany().HasForeignKey(po => po.OrderId);
                entity.HasOne<Pizza>().WithMany().HasForeignKey(po => po.PizzaId);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
