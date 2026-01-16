using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemExtraController : ControllerBase
    {
        private readonly IOrderItemExtraService _orderItemExtraService;

        public OrderItemExtraController(IOrderItemExtraService orderItemExtraService)
        {
            _orderItemExtraService = orderItemExtraService;
        }

        [HttpPut("extras")]
        public async Task<IActionResult> ManageOrderItemExtra([FromBody] ManageOrderItemExtraRequestDto dto, CancellationToken cancellationToken)
        {
            var response = await _orderItemExtraService.ManageOrderItemExtra(dto, cancellationToken);
            return Ok(response);
        }
    }
}
