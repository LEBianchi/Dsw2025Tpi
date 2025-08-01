using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
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
        try
        {
            var created = await _service.CreateOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = created.OrderId }, created);
        }
        catch (InvalidOrderDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (CustomerNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ProductNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al crear la orden.");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetOrders(
           [FromQuery] OrderStatus? status,
           [FromQuery] Guid? customerId,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
    {
        try
        {
            var orders = await _service.GetOrders(status, customerId, pageNumber, pageSize);
            if (orders == null || !orders.Any())
                return NoContent();

            return Ok(orders);
        }
        catch (Exception)
        {
            return Problem("Error inseperado");
        }
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        try
        {
            var order = await _service.GetOrderById(id);

            if (order == null)
            {
                return NotFound($"No se encontró una orden con el ID: {id}");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {

            return Problem($"Error inesperado al obtener la orden: {ex.Message}");
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")] 
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateStatus request)
    {
        try
        {
            var updated = await _service.UpdateOrderStatus(id, request.NewStatus);
            return Ok(updated);
        }
        catch (OrderNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOrderStatusException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al actualizar el estado de la orden.");
        }
    }

}
