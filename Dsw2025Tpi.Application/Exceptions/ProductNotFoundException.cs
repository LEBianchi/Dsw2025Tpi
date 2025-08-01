

namespace Dsw2025Tpi.Application.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(Guid productId)
            : base($"Producto con ID {productId} no encontrado o inactivo.") { }
    }
}
