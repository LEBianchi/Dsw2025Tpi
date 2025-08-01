using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(Guid orderId)
            : base($"No se encontró una orden con ID {orderId}.") { }
    }
}
