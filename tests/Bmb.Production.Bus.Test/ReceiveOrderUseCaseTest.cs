using AutoFixture;
using Bmb.Production.Application;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Bus.Test;

[TestSubject(typeof(ReceiveOrderUseCase))]
public class ReceiveOrderUseCaseTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository = new();

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

        var useCase = new ReceiveOrderUseCase(_mockKitchenOrderRepository.Object);

        // Act
        await useCase.ExecuteAsync(orderId);

        // Assert
        _mockKitchenOrderRepository.Verify(r => r.GetAsync(orderId, default), Times.Once);
        _mockKitchenOrderRepository.Verify(r => r.EnqueueOrderAsync(order, default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var mockKitchenOrderRepository = new Mock<IKitchenOrderRepository>();
        mockKitchenOrderRepository.Setup(r => r.GetAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((KitchenOrderDto?)null)
            .Verifiable();

        var useCase = new ReceiveOrderUseCase(mockKitchenOrderRepository.Object);

        // Act
        await useCase.ExecuteAsync(Guid.NewGuid());

        // Assert
        mockKitchenOrderRepository.Verify(r => r.GetAsync(It.IsAny<Guid>(), default), Times.Once);
        mockKitchenOrderRepository.Verify(r => r.EnqueueOrderAsync(It.IsAny<KitchenOrderDto>(), default), Times.Never);
    }
}