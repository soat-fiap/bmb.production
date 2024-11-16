using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Bmb.Production.Api.Auth;
using Bmb.Production.Api.Extensions;
using Bmb.Production.Api.Model;
using Bmb.Production.Application.Dtos;
using Bmb.Production.Controllers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bmb.Production.Api.Controllers;

/// <summary>
/// Kitchen controller
/// </summary>
/// <param name="kitchenOrderService">Kitchen orders service</param>
[Route("api/kitchen/orders")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class KitchenOrdersController(IKitchenOrderService kitchenOrderService) : ControllerBase
{
    /// <summary>
    /// Retrieves all kitchen orders.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of kitchen orders.</returns>
    [HttpGet]
    [Authorize(Roles = BmbRoles.Kitchen)]
    [ProducesResponseType<KitchenQueueResponse>(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var orders = await kitchenOrderService.GetAllOrdersAsync(cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Updates the status of a kitchen order.
    /// </summary>
    /// <param name="orderId">The ID of the order to update.</param>
    /// <param name="request">The request containing the new status of the order.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPatch]
    [Route("{orderId:guid}/status")]
    [Authorize(Roles = BmbRoles.Kitchen)]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateStatus(Guid orderId,
        [FromBody] [Required] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        if (await kitchenOrderService.UpdateOrderStatusAsync(orderId, request.Status, cancellationToken))
        {
            return NoContent();
        }

        return new JsonResult("Order status not updated.", new JsonSerializerOptions { WriteIndented = true })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    /// <summary>
    /// Retrieves the next kitchen order.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IActionResult"/> containing the next kitchen order or no content if no order exists.</returns>
    [HttpGet]
    [Route("next")]
    [Authorize(Roles = BmbRoles.Kitchen)]
    [AllowAnonymous]
    [ProducesResponseType<NextOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetNextOrder(CancellationToken cancellationToken)
    {
        var order = await kitchenOrderService.GetNextOrderAsync(cancellationToken);
        return order is null ? NoContent() : Ok(order.ToNextOrderResponse());
    }
}