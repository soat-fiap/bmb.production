using Bmb.Production.Api.Controllers;
using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Controllers;
using JetBrains.Annotations;
using Moq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;

namespace Bmb.Production.Api.Test.Controllers;

[TestSubject(typeof(KitchenController))]
public class KitchenControllerTest
{
    private readonly Mock<IGetKitchenLineUseCase> _getKitchenLineUseCaseMock;
    private readonly KitchenOrderService _kitchenOrderService;

    public KitchenControllerTest()
    {
        _getKitchenLineUseCaseMock = new Mock<IGetKitchenLineUseCase>();
        _kitchenOrderService = new KitchenOrderService(_getKitchenLineUseCaseMock.Object);
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

        using (new AssertionScope())
        {
            _getKitchenLineUseCaseMock.VerifyAll();
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldThrowException_WhenUseCaseThrowsException()
    {
        _getKitchenLineUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"))
            .Verifiable();

        Func<Task> act = async () => await _kitchenOrderService.GetAllOrdersAsync();

        using (new AssertionScope())
        {
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Test exception");
            _getKitchenLineUseCaseMock.VerifyAll();
        }
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnEmptyResponse_WhenNoOrdersExist()
    {
        var expectedResponse = new KitchenQueueResponse(
            new List<string>(),
            new List<string>(),
            new List<string>()
        );

        _getKitchenLineUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        var result = await _kitchenOrderService.GetAllOrdersAsync();

        using (new AssertionScope())
        {
            result.Should().BeEquivalentTo(expectedResponse);
            _getKitchenLineUseCaseMock.VerifyAll();
        }
    }

    [Fact]
    public async Task Get_ShouldReturnOkWithKitchenQueueResponse_WhenOrdersExist()
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

        var controller = new KitchenController(_kitchenOrderService);

        var result = await controller.Get(CancellationToken.None);

        using (new AssertionScope())
        {
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
            _getKitchenLineUseCaseMock.VerifyAll();
        }
    }

    [Fact]
    public async Task Get_ShouldReturnOkWithEmptyResponse_WhenNoOrdersExist()
    {
        var expectedResponse = new KitchenQueueResponse(
            new List<string>(),
            new List<string>(),
            new List<string>()
        );

        _getKitchenLineUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        var controller = new KitchenController(_kitchenOrderService);

        var result = await controller.Get(CancellationToken.None);

        using (new AssertionScope())
        {
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
            _getKitchenLineUseCaseMock.VerifyAll();
        }
    }

    [Fact]
    public async Task Get_ShouldThrowException_WhenUseCaseThrowsException()
    {
        _getKitchenLineUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"))
            .Verifiable();

        var controller = new KitchenController(_kitchenOrderService);

        Func<Task> act = async () => await controller.Get(CancellationToken.None);

        using (new AssertionScope())
        {
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Test exception");
            _getKitchenLineUseCaseMock.VerifyAll();
        }
    }
}