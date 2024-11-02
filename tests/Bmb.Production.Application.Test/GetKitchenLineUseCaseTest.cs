using System.Collections.Immutable;
using AutoFixture;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Application.Test;

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

        var receivedOrders = CreateOrders(fixture, KitchenOrderStatus.Queued);
        var inPreparationOrders = CreateOrders(fixture, KitchenOrderStatus.Preparing);
        var readyOrders = CreateOrders(fixture, KitchenOrderStatus.Ready);

        _mockKitchenOrderRepository.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(receivedOrders.Concat(inPreparationOrders).Concat(readyOrders).ToImmutableList())
            .Verifiable();

        // Act
        var response = await _useCase.ExecuteAsync();

        // Assert
        using var scope = new AssertionScope();
        response.Queued.Should().BeEquivalentTo(receivedOrders.Select(i => i.OrderTrackingCode));
        response.InPreparation.Should().BeEquivalentTo(inPreparationOrders.Select(i => i.OrderTrackingCode));
        response.Ready.Should().BeEquivalentTo(readyOrders.Select(i => i.OrderTrackingCode));

        _mockKitchenOrderRepository.Verify(r => r.GetAllAsync(default), Times.Once);

        ImmutableList<KitchenOrderDto> CreateOrders(Fixture fixt, KitchenOrderStatus? status)
        {
            return fixt.Build<KitchenOrderDto>()
                .With(dto => dto.Status, status)
                .CreateMany()
                .ToImmutableList();
        }
    }
}