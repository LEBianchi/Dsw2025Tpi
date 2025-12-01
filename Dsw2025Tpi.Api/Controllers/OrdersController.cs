using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrdersManagementService _service;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrdersManagementService service, ILogger<OrdersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        _logger.LogInformation("Recibida solicitud POST /api/orders para crear una nueva orden.");
        var created = await _service.CreateOrder(request);
        return CreatedAtAction(nameof(GetOrderById), new { id = created.OrderId }, created);
    }

        [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrderFilter filter)
    {
        _logger.LogInformation("Recibida solicitud GET /api/orders con filtros.");

        var result = await _service.GetOrders(filter);

        // Devolvemos Ok con el resultado (aunque esté vacío, devuelve objeto con total 0)
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        _logger.LogInformation("Recibida solicitud GET /api/orders/{OrderId}", id);
        var order = await _service.GetOrderById(id);
        return Ok(order);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string newStatus) // Ojo: simplifiqué a string, o usá tu DTO UpdateStatus
    {
        _logger.LogInformation("Recibida solicitud PUT /api/orders/{OrderId}/status", id);
        var updated = await _service.UpdateOrderStatus(id, newStatus);
        return Ok(updated);
    }
}
