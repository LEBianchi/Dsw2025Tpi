using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Dsw2025Tpi.Api.Controllers;


[ApiController]
[Route("/api/products")]

public class ProductsController : ControllerBase
{
    private readonly ProductsManagementService _service;
    
    public ProductsController(ProductsManagementService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody]ProductModel.Request request)
    {
        try
        {
            var product = await _service.AddProduct(request);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id });
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch(ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al guardar");
        }
        
    }

}
