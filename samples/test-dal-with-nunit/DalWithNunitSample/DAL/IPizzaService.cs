using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IPizzaService
    {
        Task<List<Pizza>> GetAll();

        Task<Pizza> GetById(int id);

        Task Create(Pizza pizza);

        Task Update(Pizza pizza);

        Task Delete(int id);
    }
}
