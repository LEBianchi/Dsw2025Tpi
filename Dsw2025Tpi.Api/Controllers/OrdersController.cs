using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrdersManagementService _service;

    public OrdersController(OrdersManagementService service)
    {
        _service = service;
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        var created = await _service.CreateOrder(request);
        return CreatedAtAction(nameof(GetOrderById), new { id = created.OrderId }, created);
    }


    [HttpGet]
    public async Task<IActionResult> GetOrders(
           [FromQuery] OrderStatus? status,
           [FromQuery] Guid? customerId,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
    {
        var orders = await _service.GetOrders(status, customerId, pageNumber, pageSize);
        if (orders == null)
        {
            return NoContent();
        }
        return Ok(orders);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _service.GetOrderById(id);
        return Ok(order);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")] 
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateStatus request)
    {
        var updated = await _service.UpdateOrderStatus(id, request.NewStatus);
        return Ok(updated);
    }

}
