using Microsoft.AspNetCore.Mvc;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPatch("Manage")]
    public async Task<IActionResult> ManageOrder([FromBody] OrderManageRequestDto dto, CancellationToken cancellationToken)
    {
        var response = await _orderService.ManageOrder(dto, cancellationToken);
        return Ok(response);
    }
}

