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

    [HttpGet()]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _service.GetProducts();
        if (products == null || !products.Any()) return NoContent();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.Request request)
    {
        try
        {
            var product = await _service.AddProduct(request);

            // Retorna 201 Created + la ubicación del nuevo recurso
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al guardar");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request request)
    {
        try
        {
            var updated = await _service.UpdateProduct(id, request);
            return Ok(updated);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(knf.Message); 
        }
        catch (ApplicationException de)        
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al actualizar");
        }
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> DisableProduct(Guid id)
    {
        try
        {
            await _service.DisableProduct(id);
            
            return NoContent();
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(knf.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al inhabilitar el producto");
        }
    }

}
