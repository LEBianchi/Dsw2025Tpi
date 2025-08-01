
namespace Dsw2025Tpi.Application.Exceptions
{
    public class InvalidOrderStatusException : Exception
    {
        public InvalidOrderStatusException(string status)
            : base($"Estado de orden no válido: {status}.") { }
    }
}
