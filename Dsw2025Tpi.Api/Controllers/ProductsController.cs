using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Api.Controllers;


[ApiController]
[Authorize]
[Route("/api/products")]

public class ProductsController : ControllerBase
{
    private readonly ProductsManagementService _service;
    private readonly ILogger<ProductsController> _logger;       
    public ProductsController(ProductsManagementService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    
    [AllowAnonymous]
    [HttpGet()]
    public async Task<IActionResult> GetProducts()
    {
        _logger.LogInformation("Recibida solicitud GET /api/products para obtener todos los productos.");
        var products = await _service.GetProducts();
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        _logger.LogInformation("Recibida solicitud GET /api/products/{ProductId}", id);
        var product = await _service.GetProductById(id);
        return Ok(product);
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.Request request)
    {
        _logger.LogInformation("Recibida solicitud POST /api/products para agregar un nuevo producto.");
        var created = await _service.AddProduct(request);
        return CreatedAtAction(nameof(GetProductById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request request)
    {
        _logger.LogInformation("Recibida solicitud PUT /api/products/{ProductId}", id);
        var updated = await _service.UpdateProduct(id, request);
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> DisableProduct(Guid id)
    {
        _logger.LogInformation("Recibida solicitud PATCH /api/products/{ProductId} para deshabilitar.", id);
        await _service.DisableProduct(id);
        return NoContent();
    }

}

