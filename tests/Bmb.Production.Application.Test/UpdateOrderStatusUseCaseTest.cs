using AutoFixture;
using Bmb.Domain.Core.Events;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;
using OrderStatusChanged = Bmb.Domain.Core.Events.Notifications.OrderStatusChanged;

namespace Bmb.Production.Application.Test;

[TestSubject(typeof(UpdateOrderStatusUseCase))]
public class UpdateOrderStatusUseCaseTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository = new();
    private readonly UpdateOrderStatusUseCase _useCase;
    private readonly Mock<IDispatcher> _mockDispatcher;

    public UpdateOrderStatusUseCaseTest()
    {
        _mockDispatcher = new Mock<IDispatcher>();
        _useCase = new UpdateOrderStatusUseCase(_mockKitchenOrderRepository.Object, _mockDispatcher.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateOrderStatus_WhenOrderExists()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        var status = KitchenOrderStatus.Ready;

        _mockKitchenOrderRepository.Setup(r => r.GetAsync(order.OrderId, default))
            .ReturnsAsync(order)
            .Verifiable();
        _mockKitchenOrderRepository.Setup(r => r.UpdateStatusAsync(order.OrderId, status, default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(order.OrderId, status);

        // Assert
        _mockKitchenOrderRepository.VerifyAll();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotUpdateOrderStatus_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockKitchenOrderRepository.Setup(r => r.GetAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((KitchenOrderDto?)null)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(orderId, KitchenOrderStatus.Delivered, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        _mockKitchenOrderRepository.Verify(r => r.GetAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockKitchenOrderRepository.Verify(
            r => r.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<KitchenOrderStatus>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotify_WhenStatusIsGreaterThanQueued()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        var status = KitchenOrderStatus.Preparing;

        _mockKitchenOrderRepository.Setup(r => r.GetAsync(order.OrderId, default))
            .ReturnsAsync(order)
            .Verifiable();
        _mockKitchenOrderRepository.Setup(r => r.UpdateStatusAsync(order.OrderId, status, default))
            .Returns(Task.CompletedTask)
            .Verifiable();
        _mockDispatcher.Setup(d => d.PublishAsync(It.IsAny<OrderStatusChanged>(), default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(order.OrderId, status);

        // Assert
        _mockKitchenOrderRepository.Verify();
        _mockDispatcher.Verify(d => d.PublishAsync(It.IsAny<OrderStatusChanged>(), default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotNotify_WhenStatusIsQueued()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        var status = KitchenOrderStatus.Queued;

        _mockKitchenOrderRepository.Setup(r => r.GetAsync(order.OrderId, default))
            .ReturnsAsync(order)
            .Verifiable();
        _mockKitchenOrderRepository.Setup(r => r.UpdateStatusAsync(order.OrderId, status, default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(order.OrderId, status);

        // Assert
        _mockKitchenOrderRepository.VerifyAll();
        _mockDispatcher.Verify(d => d.PublishAsync(It.IsAny<OrderStatusChanged>(), default), Times.Never);
    }
}