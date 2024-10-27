using AutoFixture;
using Bmb.Production.Application;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Bus.Test;

[TestSubject(typeof(ReplicateOrderUseCase))]
public class ReplicateOrderUseCaseTest
{
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository;
    private readonly ReplicateOrderUseCase _useCase;

    public ReplicateOrderUseCaseTest()
    {
        _mockKitchenOrderRepository = new Mock<IKitchenOrderRepository>();
        _useCase = new ReplicateOrderUseCase(_mockKitchenOrderRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSaveOrderReplica_WhenOrdersDoesNotExist()
    {
        // Arrange
        var fixture = new Fixture();
        var order = fixture.Create<KitchenOrderDto>();
        var x = default(KitchenOrderDto);
        _mockKitchenOrderRepository.Setup(r => r.GetAsync(order.OrderId, default))
            .ReturnsAsync(default(KitchenOrderDto))
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(order);

        // Assert
        using var scope = new AssertionScope();
        _mockKitchenOrderRepository.VerifyAll();
        _mockKitchenOrderRepository.Verify(r => r.SaveAsync(It.IsAny<KitchenOrderDto>(), default), Times.Once);
    }
    
    [Fact]
    public async Task ExecuteAsync_ShouldNotSaveOrderReplica_WhenOrdersAlreadyNotExist()
    {
        // Arrange
        var fixture = new Fixture();
        var order = fixture.Create<KitchenOrderDto>();
        _mockKitchenOrderRepository.Setup(r => r.GetAsync(order.OrderId, default))
            .ReturnsAsync(order)
            .Verifiable();

        // Act
        await _useCase.ExecuteAsync(order);

        // Assert
        using var scope = new AssertionScope();
        _mockKitchenOrderRepository.Verify(r => r.SaveAsync(It.IsAny<KitchenOrderDto>(), default), Times.Never);
        _mockKitchenOrderRepository.VerifyAll();
    }
}