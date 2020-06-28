using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL
{
    public interface IPizzaService
    {
        Task OrderPizza(string customer, List<int> pizzaIds);

        Task RemoveOrder(int id);
    }
}
