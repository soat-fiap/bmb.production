using System.ComponentModel.DataAnnotations;
using Bmb.Production.Core.Model;

namespace Bmb.Production.Api.Model;

/// <summary>
/// Request to update the status of an order
/// </summary>
/// <param name="Status"></param>
public record UpdateOrderStatusRequest([Required]KitchenOrderStatus Status);