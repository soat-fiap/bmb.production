using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Production.Controllers.Test;

[TestSubject(typeof(KitchenOrderService))]
public class KitchenOrderServiceTest
{
    private readonly Mock<IGetKitchenLineUseCase> _getKitchenLineUseCaseMock;
    private readonly KitchenOrderService _kitchenOrderService;

    public KitchenOrderServiceTest()
    {
        _getKitchenLineUseCaseMock = new Mock<IGetKitchenLineUseCase>();
        _kitchenOrderService = new KitchenOrderService(_getKitchenLineUseCaseMock.Object,
            Mock.Of<IUpdateOrderStatusUseCase>(),
            Mock.Of<IGetNextOrderUseCase>());
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnKitchenQueueResponse_WhenCalled()
    {
        var expectedResponse = new KitchenQueueResponse(
            new List<string> { "Order1" },
            new List<string> { "Order2" },
            new List<string> { "Order3" }
        );

        _getKitchenLineUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        var result = await _kitchenOrderService.GetAllOrdersAsync();

        result.Should().BeEquivalentTo(expectedResponse);
    }
}