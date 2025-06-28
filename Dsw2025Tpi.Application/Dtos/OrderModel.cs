using System;
using System.Collections.Generic;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Application.Dtos;
namespace Dsw2025Tpi.Application.Dtos;

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
    List<OrderItemResponse> Items
);

