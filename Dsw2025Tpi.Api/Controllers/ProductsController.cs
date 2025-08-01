using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Dsw2025Tpi.Application.Exceptions;

namespace Dsw2025Tpi.Api.Controllers;


[ApiController]
[Authorize]
[Route("/api/products")]

public class ProductsController : ControllerBase
{
    private readonly ProductsManagementService _service;
    
    public ProductsController(ProductsManagementService service)
    {
        _service = service;
    }

    
    [AllowAnonymous]
    [HttpGet()]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _service.GetProducts();
        if (products == null || !products.Any())
            return NoContent();
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.Request request)
    {
        try
        {
            var product = await _service.AddProduct(request);

            // Retorna 201 Created + la ubicación del nuevo recurso
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DuplicatedEntityException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Se produjo un error al guardar");
        }

    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request request)
    {
        try
        {
            var updated = await _service.UpdateProduct(id, request);
            return Ok(updated);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
        // CAMBIO: Captura DuplicatedEntityException
        catch (DuplicatedEntityException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return Problem("Se produjo un error al actualizar");
        }
    }

    [Authorize(Roles = "Admin")]
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

