using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string productName, int available, int requested)
            : base($"El producto '{productName}' no tiene suficiente stock. Stock actual: {available}, solicitado: {requested}.") { }
    }
}
