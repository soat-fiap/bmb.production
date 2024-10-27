using System.Collections.Immutable;
using AutoFixture;
using Bmb.Production.Application;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Bus.Test;

[TestSubject(typeof(GetKitchenLineUseCase))]
public class GetKitchenLineUseCaseTest
{
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository;
    private readonly GetKitchenLineUseCase _useCase;

    public GetKitchenLineUseCaseTest()
    {
        _mockKitchenOrderRepository = new Mock<IKitchenOrderRepository>();
        _useCase = new GetKitchenLineUseCase(_mockKitchenOrderRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnResponse_WhenOrdersExist()
    {
        // Arrange
        var fixture = new Fixture();
        var receivedOrders = fixture.CreateMany<string>().ToImmutableList();
        var inPreparationOrders = fixture.CreateMany<string>().ToImmutableList();
        var readyOrders = fixture.CreateMany<string>().ToImmutableList();

        _mockKitchenOrderRepository.Setup(r => r.GetAllAsync(KitchenQueue.Received, default))
            .ReturnsAsync(receivedOrders)
            .Verifiable();

        _mockKitchenOrderRepository.Setup(r => r.GetAllAsync(KitchenQueue.InPreparation, default))
            .ReturnsAsync(inPreparationOrders)
            .Verifiable();

        _mockKitchenOrderRepository.Setup(r => r.GetAllAsync(KitchenQueue.Ready, default))
            .ReturnsAsync(readyOrders)
            .Verifiable();

        // Act
        var response = await _useCase.ExecuteAsync(default);

        // Assert
        using var scope = new AssertionScope();
        response.Received.Should().BeEquivalentTo(receivedOrders);
        response.InPreparation.Should().BeEquivalentTo(inPreparationOrders);
        response.Ready.Should().BeEquivalentTo(readyOrders);

        _mockKitchenOrderRepository.Verify(r => r.GetAllAsync(It.IsAny<KitchenQueue>(), default), Times.Exactly(3));
    }
}