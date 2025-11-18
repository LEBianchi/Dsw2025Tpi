using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
    public async Task<IActionResult> GetProducts([FromQuery] ProductModel.FilterProduct filter)
    {
        _logger.LogInformation("Recibida solicitud GET /api/products para obtener todos los productos.");
       
        var result =  await _service.GetProducts(filter);

        if (result.ProductItems == null || !result.ProductItems.Any())
        {
            _logger.LogWarning("No se encontraron productos activos.");
            return NoContent();
        }
        return Ok(result);
    }


    [HttpGet("admin")] 
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetAuthProducts([FromQuery] ProductModel.FilterProduct request)
    {
        _logger.LogInformation("Admin solicitando productos (incluyendo inactivos).");

        var result = await _service.GetProducts(request, includeInactive: true);

        if (result.ProductItems == null || !result.ProductItems.Any())
        {
            
            Response.Headers.Append("X-Message", "There are no products found");
            return NoContent();
        }

        return Ok(result);
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

