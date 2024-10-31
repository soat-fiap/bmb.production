using Bmb.Production.Api.Auth;
using Bmb.Production.Application.Dtos;
using Bmb.Production.Controllers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bmb.Production.Api.Controllers;

/// <summary>
/// Kitchen controller
/// </summary>
/// <param name="kitchenOrderService">Kitchen orders service</param>
[Route("api/[controller]")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class KitchenController(IKitchenOrderService kitchenOrderService) : ControllerBase
{
    /// <summary>
    /// Get kitchen orders
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    // [Authorize(Roles = BmbRoles.Kitchen)]
    [AllowAnonymous]
    public async Task<ActionResult<KitchenQueueResponse>> Get(CancellationToken cancellationToken)
    {
        var orders = await kitchenOrderService.GetAllOrdersAsync(cancellationToken);
        return Ok(orders);
    }
}