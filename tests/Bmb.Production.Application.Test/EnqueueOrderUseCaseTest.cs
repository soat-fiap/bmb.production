using AutoFixture;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Application.Test;

[TestSubject(typeof(EnqueueOrderUseCase))]
public class EnqueueOrderUseCaseTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository = new();
    private readonly Mock<IUpdateOrderStatusUseCase> _mockUpdateOrderStatusUseCase = new();
    private readonly EnqueueOrderUseCase _useCase;

    public EnqueueOrderUseCaseTest()
    {
        _useCase = new EnqueueOrderUseCase(_mockKitchenOrderRepository.Object, _mockUpdateOrderStatusUseCase.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldEnqueueOrder_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = _fixture.Create<KitchenOrderDto>();

        _mockKitchenOrderRepository.Setup(r => r.GetAsync(orderId, default))
            .ReturnsAsync(order)
            .Verifiable();
        _mockKitchenOrderRepository.Setup(r => r.EnqueueOrderAsync(order, default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(orderId);

        // Assert
        _mockKitchenOrderRepository.Verify(r => r.GetAsync(orderId, default), Times.Once);
        _mockKitchenOrderRepository.Verify(r => r.EnqueueOrderAsync(order, default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockKitchenOrderRepository.Setup(r => r.GetAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((KitchenOrderDto?)null)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(orderId, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        _mockKitchenOrderRepository.Verify(r => r.GetAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockKitchenOrderRepository.Verify(r => r.EnqueueOrderAsync(It.IsAny<KitchenOrderDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}