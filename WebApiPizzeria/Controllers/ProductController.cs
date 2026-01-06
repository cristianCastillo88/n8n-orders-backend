using Microsoft.AspNetCore.Mvc;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableProducts()
    {
        var products = await _productService.GetAvalaibleProducts();
        return Ok(products);
    }
}

