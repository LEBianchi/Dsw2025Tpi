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

    [HttpPost()]

    public async Task<IActionResult> AddProduct([FromBody]ProductModel.Request request)
    {
        try
        {
            var product = await _service.AddProduct(request);
            return Ok(product);
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
