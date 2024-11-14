using AutoFixture;
using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Controllers.Test;

[TestSubject(typeof(KitchenOrderService))]
public class KitchenOrderServiceTest
{
    private readonly Mock<IGetKitchenLineUseCase> _getKitchenLineUseCaseMock = new();
    private readonly Mock<IGetNextOrderUseCase> _mockGetNextOrderUseCase = new();
    private readonly KitchenOrderService _kitchenOrderService;

    public KitchenOrderServiceTest()
    {
        _kitchenOrderService = new KitchenOrderService(_getKitchenLineUseCaseMock.Object,
            Mock.Of<IUpdateOrderStatusUseCase>(),
            _mockGetNextOrderUseCase.Object);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnKitchenQueueResponse_WhenCalled()
    {
        var expectedResponse = new KitchenQueueResponse(
            new List<KitchenQueueItem> { new(Guid.NewGuid(), "Order1") },
            new List<KitchenQueueItem> { new(Guid.NewGuid(), "Order2") },
            new List<KitchenQueueItem> { new(Guid.NewGuid(), "Order3") }
        );

        _getKitchenLineUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        var result = await _kitchenOrderService.GetAllOrdersAsync();

        result.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldReturnTrue_WhenStatusIsUpdated()
    {
        var orderId = Guid.NewGuid();
        var status = KitchenOrderStatus.Preparing;

        var result = await _kitchenOrderService.UpdateOrderStatusAsync(orderId, status);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetNextOrderAsync_ShouldReturnOrder_WhenOrderExists()
    {
        var expectedOrder = new Fixture().Create<KitchenOrderDto>();
        _mockGetNextOrderUseCase
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder)
            .Verifiable();

        var result = await _kitchenOrderService.GetNextOrderAsync();

        result.Should().BeEquivalentTo(expectedOrder);
    }
}