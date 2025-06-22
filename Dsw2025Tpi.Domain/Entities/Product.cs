using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product : EntityBase
    {
       
       
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal CurrentUnitPrice { get; set; }

        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
   
        public Product() { }

        public Product(string sku, string name, decimal price, int stock, string? description = null)
        {
            Sku = sku;
            Name = name;
            CurrentUnitPrice = price;
            StockQuantity = stock;
            Description = description;
            IsActive = true;
        }
    }
}
