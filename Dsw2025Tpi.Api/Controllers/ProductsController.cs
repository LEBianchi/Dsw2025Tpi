using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Authorization;


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
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        return Ok(product);
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.Request request)
    {
     var created = await _service.AddProduct(request);
        return CreatedAtAction(nameof(GetProductById), new { id = created.Id }, created);

    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request request)
    {
        var updated = await _service.UpdateProduct(id, request);
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> DisableProduct(Guid id)
    {
        await _service.DisableProduct(id);
        return NoContent();
    }

}

