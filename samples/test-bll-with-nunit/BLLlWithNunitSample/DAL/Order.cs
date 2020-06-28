using System;

namespace DAL
{
    public class Order
    {
        public int Id { get; set; }

        public string Customer { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
