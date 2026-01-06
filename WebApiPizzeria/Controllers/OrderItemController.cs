using Microsoft.AspNetCore.Mvc;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpPost("SaveRange")]
    public async Task<IActionResult> SaveRange(OrderItemPostDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderItemService.SaveRange(dto, cancellationToken);
        return Ok(response);
    }
}

