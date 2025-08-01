using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService
{
    private readonly IRepository _repository;
    private readonly ILogger<OrdersManagementService> _logger;
    public OrdersManagementService(IRepository repository, ILogger<OrdersManagementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }



    public async Task<OrderResponse> GetOrderById(Guid id)
    {
        _logger.LogInformation("Buscando orde con ID: {OrderID}", id);
        var order = await _repository.GetById<Order>(id, "OrderItems.Product");

        
        if (order == null)
        {
            _logger.LogWarning("Orden con ID: {OrderId} no encontrada.", id);
            throw new OrderNotFoundException(id);
        }

        var orderItemResponses = order.OrderItems.Select(oi => new OrderItemResponse(
            oi.ProductId ?? Guid.Empty,
            oi.Product?.Name,
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
    public async Task<IEnumerable<OrderResponse>> GetOrders(
       OrderStatus? status,
       Guid? customerId,
       int pageNumber,
       int pageSize)
    {
        _logger.LogInformation("Buscando ordenes con filtros: Status={Status}, CustomerId={CustomerId}, Page={Page}, Size={PageSize}", status, customerId, pageNumber, pageSize);
        var query = await _repository.GetAll<Order>("OrderItems.Product");

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        var paginated = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return paginated.Select(order => new OrderResponse(
          order.Id,
          order.Date,
          order.ShippingAddress,
          order.BillingAddress,
          order.Notes,
          order.TotalAmount,
          order.Status,
            order.OrderItems.Select(oi => new OrderItemResponse(
                oi.ProductId ?? Guid.Empty,
                oi.Product?.Name ?? string.Empty,
                oi.Quantity,
                oi.UnitPrice,
                oi.Subtotal)).ToList()
        ));
    }

    public async Task<OrderResponse> CreateOrder(OrderRequest request)
    {
        _logger.LogInformation("Iniciando proceso para crear una nueva orden para el cliente ID: {CustomerId}", request.CustomerId);

        if (string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress) ||
            request.Items == null || !request.Items.Any())
        {

            throw new InvalidOrderDataException("Datos incompletos para la orden.");
        }

        var customer = await _repository.GetById<Customer>(request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Cliente con ID {CustomerId} no encontrado al intentar crear una orden.", request.CustomerId);
            throw new CustomerNotFoundException(request.CustomerId);
        }

        var orderItems = new List<OrderItem>();
        decimal total = 0m;

        foreach (var item in request.Items)
        {
            var product = await _repository.GetById<Product>(item.ProductId);

            if (product == null || !product.IsActive)
            {
                _logger.LogError("Producto con ID {ProductId} no encontrado o inactivo al crear orden.", item.ProductId);
                throw new ArgumentException($"Producto con Id={item.ProductId} no encontrado o inactivo.");
            }

            if (product.StockQuantity < item.Quantity)
            {
                _logger.LogError("Stock insuficiente para el producto ID {ProductId}. Solicitado: {Quantity}, Disponible: {Stock}", item.ProductId, item.Quantity, product.StockQuantity);
                throw new InsufficientStockException(product.Name, product.StockQuantity,item.Quantity);
            }

            _logger.LogInformation("Actualizando stock para producto ID {ProductId}. Stock anterior: {OldStock}, Nuevo stock: {NewStock}", product.Id, product.StockQuantity, product.StockQuantity - item.Quantity);
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
        _logger.LogInformation("Orden con ID {OrderId} creada exitosamente.", order.Id);

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
    public async Task<OrderResponse> UpdateOrderStatus(Guid orderId, string newStatus)
    {
        _logger.LogInformation("Intentando actualizar estado de la orden ID: {OrderId} a '{NewStatus}'", orderId, newStatus);
        var order = await _repository.GetById<Order>(orderId, nameof(Order.OrderItems), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}");

        if (order == null)
        {
            _logger.LogWarning("Intento de actualizar estado de una orden no encontrada. ID: {OrderId}", orderId);
            throw new OrderNotFoundException(orderId);
        }

        if (newStatus.Any(char.IsDigit))
        {
            _logger.LogError("Intento Actualizar orden ID {OrderId} con un numero lo cual no es valido: {InvalidStatus}", orderId, newStatus);
            throw new InvalidOrderStatusException($"El estado '{newStatus}' no puede contener números. Por favor, use uno de los siguientes: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");
        }
        if (!Enum.TryParse(newStatus, true, out OrderStatus parsedStatus) || !Enum.IsDefined(typeof(OrderStatus), parsedStatus))
        {
            _logger.LogError("Intento de actualizar orden ID {OrderId} con un estado inválido: '{InvalidStatus}'", orderId, newStatus);
            throw new InvalidOrderStatusException($"El estado '{newStatus}' no es un valor válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");
        }
        order.Status = parsedStatus;

        var updatedOrder = await _repository.Update(order);
        _logger.LogInformation("Estado de la orden ID {OrderId} actualizado a '{NewStatus}'", orderId, newStatus);
        return new OrderResponse(
            updatedOrder.Id,
            updatedOrder.Date,
            updatedOrder.ShippingAddress,
            updatedOrder.BillingAddress,
            updatedOrder.Notes,
            updatedOrder.TotalAmount,
            updatedOrder.Status,
            updatedOrder.OrderItems.Select(oi => new OrderItemResponse(
                oi.ProductId ?? Guid.Empty,
                oi.Product?.Name ?? "(sin nombre)",
                oi.Quantity,
                oi.UnitPrice,
                oi.Subtotal)).ToList()
        );
    }
}
