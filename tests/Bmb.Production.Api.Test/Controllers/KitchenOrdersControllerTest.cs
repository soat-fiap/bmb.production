using AutoFixture;
using Bmb.Production.Api.Controllers;
using Bmb.Production.Api.Extensions;
using Bmb.Production.Api.Model;
using Bmb.Production.Application.Dtos;
using Bmb.Production.Controllers.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bmb.Production.Api.Test.Controllers;

[TestSubject(typeof(KitchenOrdersController))]
public class KitchenOrdersControllerTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IKitchenOrderService> _mockKitchenOrderService = new();
    private readonly KitchenOrdersController _controller;

    public KitchenOrdersControllerTest()
    {
        _controller = new KitchenOrdersController(_mockKitchenOrderService.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOkWithOrders_WhenOrdersExist()
    {
        // Arrange
        var orders = _fixture.Create<KitchenQueueResponse>();
        _mockKitchenOrderService.Setup(s => s.GetAllOrdersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders)
            .Verifiable();

        // Act
        var result = await _controller.Get(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(orders);
        _mockKitchenOrderService.VerifyAll();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnNoContent_WhenOrderStatusIsUpdated()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var request = new UpdateOrderStatusRequest(KitchenOrderStatus.Preparing);
        _mockKitchenOrderService
            .Setup(s => s.UpdateOrderStatusAsync(orderId, request.Status, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        // Act
        var result = await _controller.UpdateStatus(orderId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockKitchenOrderService.VerifyAll();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnInternalServerError_WhenOrderStatusIsNotUpdated()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var request = new UpdateOrderStatusRequest(KitchenOrderStatus.Ready);
        _mockKitchenOrderService
            .Setup(s => s.UpdateOrderStatusAsync(orderId, request.Status, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .Verifiable();

        // Act
        var result = await _controller.UpdateStatus(orderId, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result as JsonResult;
        jsonResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        jsonResult.Value.Should().Be("Order status not updated.");
        _mockKitchenOrderService.VerifyAll();
    }

    [Fact]
    public async Task GetNextOrder_ShouldReturnOkWithOrder_WhenOrderExists()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        var expectedOrder = order.ToNextOrderResponse();
        _mockKitchenOrderService.Setup(s => s.GetNextOrderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(order)
            .Verifiable();

        // Act
        var result = await _controller.GetNextOrder(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult?>();
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(expectedOrder);
        _mockKitchenOrderService.Verify(s => s.GetNextOrderAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNextOrder_ShouldReturnNotFound_WhenNoOrderExists()
    {
        // Arrange
        _mockKitchenOrderService.Setup(s => s.GetNextOrderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((KitchenOrderDto?)null)
            .Verifiable();

        // Act
        var result = await _controller.GetNextOrder(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockKitchenOrderService.Verify(s => s.GetNextOrderAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}