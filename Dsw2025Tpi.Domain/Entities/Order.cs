using Dsw2025Tpi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {

        public DateTime Date { get; set;}
        public string? ShippingAddress { get; set;}
        public string? BillingAddress { get; set;}
        public string? Notes { get; set;}
        public decimal TotalAmount { get; set;}
        public OrderStatus Status { get; set; }
        public Guid? CustomerId { get; set;}
        public Customer? Customer { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public Order() { }

        public Order(Guid customerId, string shippingAddress, string billingAddress, string? notes = null)
        {
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Notes = notes;
            Date = DateTime.Now;
            Status = OrderStatus.PENDING;
        }

    }
}
