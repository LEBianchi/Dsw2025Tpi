
namespace Dsw2025Tpi.Application.Exceptions
{
    public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException(Guid customerId)
            : base($"No se encontró el cliente con ID {customerId}.") { }
    }
}
