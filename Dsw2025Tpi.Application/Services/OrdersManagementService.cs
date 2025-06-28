using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService
{
    private readonly IRepository _repository;

    public OrdersManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> CreateOrder(OrderRequest request)
    {
        // Validación básica
        if (string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress) ||
            request.Items == null || !request.Items.Any())
        {
            throw new ArgumentException("Datos incompletos para la orden.");
        }

        var customer = await _repository.GetById<Customer>(request.CustomerId);
        if (customer == null)
        {
            throw new ArgumentException($"No se encontró el cliente con ID {request.CustomerId}.");
        }

        var orderItems = new List<OrderItem>();
        decimal total = 0m;

        foreach (var item in request.Items)
        {
            var product = await _repository.GetById<Product>(item.ProductId);

            if (product == null || !product.IsActive)
            {
                throw new ArgumentException($"Producto con Id={item.ProductId} no encontrado o inactivo.");
            }

            if (product.StockQuantity < item.Quantity)
            {
                throw new ArgumentException(
                    $"El producto '{product.Name}' no tiene suficiente stock. Stock actual: {product.StockQuantity}, solicitado: {item.Quantity}"
                );
            }

            product.StockQuantity -= item.Quantity;
            await _repository.Update(product);

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.CurrentUnitPrice
            };

            orderItems.Add(orderItem);
            total += orderItem.Subtotal;
        }

        var order = new Order(request.CustomerId, request.ShippingAddress, request.BillingAddress, request.Notes)
        {
            TotalAmount = total,
            OrderItems = orderItems
        };

        await _repository.Add(order);

        var orderItemResponses = orderItems.Select(oi => new OrderItemResponse(
            oi.ProductId ?? Guid.Empty,
            oi.Product?.Name ?? "(sin nombre)",
            oi.Quantity,
            oi.UnitPrice,
            oi.Subtotal
        )).ToList();

        return new OrderResponse(
            order.Id,
            order.Date,
            order.ShippingAddress,
            order.BillingAddress,
            order.Notes,
            order.TotalAmount,
            order.Status,
            orderItemResponses
        );
    }
}
