using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
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
        try
        {
            var result = await _service.CreateOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.OrderId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return Problem("Error inesperado al crear la orden.");
        }
    }

    // Método opcional para que funcione el CreatedAtAction y puedas hacer pruebas con GET
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        // Esto es opcional y deberías implementarlo más adelante
        return NotFound("Este endpoint aún no está implementado.");
    }
}
