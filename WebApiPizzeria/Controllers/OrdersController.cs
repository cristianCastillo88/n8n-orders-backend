using Microsoft.AspNetCore.Mvc;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Agrega o actualiza un item en un pedido
    /// </summary>
    /// <param name="request">Datos del item a agregar</param>
    /// <returns>Respuesta con el total actualizado</returns>
    [HttpPost("add-item")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _orderService.AddItemToOrderAsync(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Elimina un item específico de un pedido
    /// </summary>
    /// <param name="orderId">ID de la orden</param>
    /// <param name="productId">ID del producto a eliminar</param>
    /// <returns>Respuesta con el total actualizado</returns>
    [HttpDelete("{orderId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(int orderId, int productId)
    {
        var response = await _orderService.RemoveItemFromOrderAsync(orderId, productId);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Cancela un pedido (solo si está en estado ABIERTO)
    /// </summary>
    /// <param name="orderId">ID de la orden a cancelar</param>
    /// <returns>Respuesta de éxito o error</returns>
    [HttpPut("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var response = await _orderService.CancelOrderAsync(orderId);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
