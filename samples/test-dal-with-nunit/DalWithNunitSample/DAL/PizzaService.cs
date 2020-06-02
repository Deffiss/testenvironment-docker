using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class PizzaService : IPizzaService
    {
        private readonly PizzaContext _dbContext;

        public PizzaService(PizzaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Pizza>> GetAll()
        {
            return _dbContext.Pizzas.ToListAsync();
        }

        public Task<Pizza> GetById(int id)
        {
            return _dbContext.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Create(Pizza pizza)
        {
            _dbContext.Pizzas.Add(pizza);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Pizza pizza)
        {
            var pizzaToUpdate = await _dbContext.Pizzas.FirstOrDefaultAsync(x => x.Id == pizza.Id);
            pizzaToUpdate.Name = pizza.Name;

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            _dbContext.Pizzas.Remove(await _dbContext.Pizzas.FirstOrDefaultAsync(x => x.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
