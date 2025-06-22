using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }  
        public decimal Subtotal => Quantity * UnitPrice;
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid? ProductId { get; set; }    
        public Product? Product { get; set; }
   
        public OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
