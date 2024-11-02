using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;
namespace Bmb.Production.Application.Test;

[TestSubject(typeof(GetNextOrderUseCase))]
public class GetNextOrderUseCaseTest
{
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository = new();
    private readonly GetNextOrderUseCase _useCase;

    public GetNextOrderUseCaseTest()
    {
        _useCase = new GetNextOrderUseCase(_mockKitchenOrderRepository.Object);
    }
    
    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenNoOrderExists()
    {
        _mockKitchenOrderRepository.Setup(r => r.GetNextOrderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((KitchenOrderDto?)null)
            .Verifiable();

        var result = await _useCase.ExecuteAsync();

        result.Should().BeNull();
        _mockKitchenOrderRepository.Verify(r => r.GetNextOrderAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenRepositoryThrowsException()
    {
        _mockKitchenOrderRepository.Setup(r => r.GetNextOrderAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Repository error"))
            .Verifiable();

        await Assert.ThrowsAsync<Exception>(() => _useCase.ExecuteAsync());

        _mockKitchenOrderRepository.Verify(r => r.GetNextOrderAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}