
namespace Dsw2025Tpi.Application.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(Guid orderId)
            : base($"No se encontró una orden con ID {orderId}.") { }
    }
}
