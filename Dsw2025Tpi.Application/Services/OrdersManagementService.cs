using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Interfaces;


namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService
{
    private readonly IRepository _repository;

    public OrdersManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse?> GetOrderById(Guid id)
    {
        var order = await _repository.GetById<Order>(id, "OrderItems.Product");
           
        if (order == null)
        {
            return null;
        }
        var orderItemResponses = order.OrderItems.Select(oi => new OrderItemResponse(
            oi.ProductId ?? Guid.Empty, 
            oi.Product?.Name, //se podria poner ??"(sin nombre)" si es null
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

        if (string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress) ||
            request.Items == null || !request.Items.Any())
        {
            throw new InvalidOrderDataException("Datos incompletos para la orden.");
        }

        var customer = await _repository.GetById<Customer>(request.CustomerId);
        if (customer == null)
        {
            throw new CustomerNotFoundException(request.CustomerId);
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
                throw new InsufficientStockException(product.Name, product.StockQuantity,item.Quantity);
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
    public async Task<OrderResponse> UpdateOrderStatus(Guid orderId, string newStatus)
    {
        var order = await _repository.GetById<Order>(orderId, nameof(Order.OrderItems), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}");

        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }

        if (newStatus.Any(char.IsDigit))
        {
            throw new InvalidOrderStatusException($"El estado '{newStatus}' no puede contener números. Por favor, use uno de los siguientes: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");
        }
        if (!Enum.TryParse(newStatus, true, out OrderStatus parsedStatus) || !Enum.IsDefined(typeof(OrderStatus), parsedStatus))
        {
            throw new InvalidOrderStatusException($"El estado '{newStatus}' no es un valor válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");
        }
        order.Status = parsedStatus;

        var updatedOrder = await _repository.Update(order);

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
