using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public class InvalidOrderStatusException : Exception
    {
        public InvalidOrderStatusException(string status)
            : base($"Estado de orden no válido: {status}.") { }
    }
}
