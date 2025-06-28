using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions
{
    public class DuplicatedEntityException : Exception
    {
        public DuplicatedEntityException(string message) : base(message)
        {
        }
    }
}
