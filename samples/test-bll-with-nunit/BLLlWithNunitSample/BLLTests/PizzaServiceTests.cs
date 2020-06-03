using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BLLTests
{
    public class PizzaServiceTests : TestBase
    {
        private IPizzaService _pizzaService;

        [OneTimeSetUp]
        public void Initialization()
        {
            _pizzaService = new PizzaService(DbContext);
        }

        [Test]
        public async Task OrderPizza_WhenOrderSpecified_ShouldCreateOrder()
        {
            // Arrange
            var pepperoni = new Pizza { Name = "Pepperoni" };
            var napoli = new Pizza { Name = "Spacca Napoli" };

            DbContext.Pizzas.AddRange(new Pizza[] { pepperoni, napoli });
            await DbContext.SaveChangesAsync();

            // Act
            await _pizzaService.OrderPizza("John Smith", new List<Pizza> { pepperoni, napoli });

            // Assert
            var order = await DbContext.Orders.FirstOrDefaultAsync();
            var pizzaOrders = await DbContext.PizzaOrders.ToListAsync();

            order.Should().BeEquivalentTo(
                new Order
            {
                Customer = "John Smith",
                CreatedAt = DateTime.UtcNow.Date
            }, options => options.Excluding(x => x.Id));
            pizzaOrders.Should().BeEquivalentTo(new List<PizzaOrder>
            {
                new PizzaOrder { OrderId = order.Id, PizzaId = pepperoni.Id },
                new PizzaOrder { OrderId = order.Id, PizzaId = napoli.Id }
            });
        }

        [Test]
        public async Task RemoveOrder_WhenOrderIdSpecified_ShouldRemoveOrder()
        {
            // Arrange
            var pepperoni = new Pizza { Name = "Pepperoni" };
            var napoli = new Pizza { Name = "Spacca Napoli" };
            var order = new Order { CreatedAt = DateTime.UtcNow.Date, Customer = "Bart Simpson" };

            DbContext.Pizzas.AddRange(new Pizza[] { pepperoni, napoli });
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            DbContext.PizzaOrders.AddRange(new PizzaOrder[]
            {
                new PizzaOrder { OrderId = order.Id, PizzaId = pepperoni.Id },
                new PizzaOrder { OrderId = order.Id, PizzaId = napoli.Id }
            });
            await DbContext.SaveChangesAsync();

            // Act
            await _pizzaService.RemoveOrder(order.Id);

            // Assert
            var orders = await DbContext.Orders.ToListAsync();
            var pizzaOrders = await DbContext.PizzaOrders.ToListAsync();

            orders.Should().BeEmpty();
            pizzaOrders.Should().BeEmpty();
        }
    }
}
