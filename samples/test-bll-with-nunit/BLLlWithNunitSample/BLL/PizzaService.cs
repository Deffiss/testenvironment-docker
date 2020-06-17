using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace BLL
{
    public class PizzaService : IPizzaService
    {
        private readonly PizzaContext _dbContext;

        public PizzaService(PizzaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OrderPizza(string customer, List<int> pizzaIds)
        {
            var order = new Order
            {
                Customer = customer,
                CreatedAt = DateTime.UtcNow.Date
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            var pizzas = await _dbContext.Pizzas.Where(x => pizzaIds.Contains(x.Id)).ToListAsync();

            _dbContext.PizzaOrders.AddRange(pizzas.Select(x => new PizzaOrder { OrderId = order.Id, PizzaId = x.Id }));
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveOrder(int id)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            var pizzaOrders = await _dbContext.PizzaOrders.Where(x => x.OrderId == order.Id).ToListAsync();

            _dbContext.PizzaOrders.RemoveRange(pizzaOrders);
            _dbContext.Orders.Remove(order);

            await _dbContext.SaveChangesAsync();
        }
    }
}
