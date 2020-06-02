using System.Threading.Tasks;
using DAL;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DALTests
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
        public async Task GetAll_WhenPizzasListExists_ShouldReturnAllPizzas()
        {
            // Arrange
            DbContext.Pizzas.AddRange(new Pizza[]
            {
                new Pizza { Name = "Pepperoni" },
                new Pizza { Name = "Spacca Napoli" },
                new Pizza { Name = "Gino Sorbilo" }
            });
            await DbContext.SaveChangesAsync();

            // Act
            var pizzas = await _pizzaService.GetAll();

            // Assert
            pizzas.Should().BeEquivalentTo(
                new Pizza[]
            {
                new Pizza { Name = "Pepperoni" },
                new Pizza { Name = "Spacca Napoli" },
                new Pizza { Name = "Gino Sorbilo" }
            }, options => options.Excluding(x => x.Id));
        }

        [Test]
        public async Task GetById_WhenPizzaWithSpecifiedIdExists_ShouldReturnFoundPizza()
        {
            // Arrange
            var pepperoni = new Pizza { Name = "Pepperoni" };
            DbContext.Add(pepperoni);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _pizzaService.GetById(pepperoni.Id);

            // Assert
            result.Should().BeEquivalentTo(new Pizza { Id = pepperoni.Id, Name = "Pepperoni" });
        }

        [Test]
        public async Task Create_WhenPizzaSpecified_ShouldCreatePizza()
        {
            // Arrange & Act
            await _pizzaService.Create(new Pizza { Name = "Pepperoni" });

            // Assert
            var result = await DbContext.Pizzas.FirstOrDefaultAsync(x => x.Name == "Pepperoni");
            result.Should().BeEquivalentTo(new Pizza { Name = "Pepperoni" }, options => options.Excluding(x => x.Id));
        }

        [Test]
        public async Task Update_WhenProvidedUpdatedPizza_ShouldUpdatePizza()
        {
            // Arrange
            var pepperoni = new Pizza { Name = "Pepperoni" };
            DbContext.Pizzas.Add(pepperoni);
            await DbContext.SaveChangesAsync();

            // Act
            pepperoni.Name = "Spacca Napoli";
            await _pizzaService.Update(pepperoni);

            // Assert
            var result = await DbContext.Pizzas.FirstOrDefaultAsync(x => x.Id == pepperoni.Id);
            result.Should().BeEquivalentTo(new Pizza { Id = pepperoni.Id, Name = "Spacca Napoli" });
        }

        [Test]
        public async Task Delete_WhenProvidedId_ShouldRemovePizza()
        {
            // Arrange
            var pepperoni = new Pizza { Name = "pepperoni" };
            DbContext.Pizzas.Add(pepperoni);
            await DbContext.SaveChangesAsync();

            // Act
            await _pizzaService.Delete(pepperoni.Id);

            // Assert
            var result = await DbContext.Pizzas.ToListAsync();
            result.Should().BeEmpty();
        }
    }
}
