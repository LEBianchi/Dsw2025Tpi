

namespace Dsw2025Tpi.Application.Dtos
{
   
    
        public record OrderItemRequest(
    Guid ProductId,
    int Quantity
);

        public record OrderItemResponse(
            Guid ProductId,
            string ProductName,
            int Quantity,
            decimal UnitPrice,
            decimal Subtotal
        );
 }

