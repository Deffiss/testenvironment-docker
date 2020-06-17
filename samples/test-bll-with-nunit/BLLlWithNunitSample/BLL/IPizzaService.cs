using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;

namespace BLL
{
    public interface IPizzaService
    {
        Task OrderPizza(string customer, List<Pizza> pizzas);

        Task RemoveOrder(int id);
    }
}
