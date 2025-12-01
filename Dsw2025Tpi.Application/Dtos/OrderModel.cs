using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Enums;

public record OrderRequest(
    Guid CustomerId,
    string ShippingAddress,
    string BillingAddress,
    string? Notes,
    List<OrderItemRequest> Items
);


public record OrderResponse(
    Guid OrderId,
    DateTime Date,
    string ShippingAddress,
    string BillingAddress,
    string? Notes,
    decimal TotalAmount,
    OrderStatus Status,
    List<OrderItemResponse> Items,
    string? ClientName 
);


public record OrderResponsePagination(
    List<OrderResponse> Items, 
    int Total                  
);


public record OrderFilter(
    string? Search,      
    string? Status,      
    int? PageNumber,     
    int? PageSize        
);