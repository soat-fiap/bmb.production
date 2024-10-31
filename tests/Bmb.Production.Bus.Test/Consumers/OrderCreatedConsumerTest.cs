using AutoFixture;
using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Bus.Consumers;
using Bmb.Production.Core.Model.Dto;
using Bmb.Test.Common;
using FluentAssertions;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Production.Bus.Test.Consumers;

[TestSubject(typeof(OrderCreatedConsumer))]
public class OrderCreatedConsumerTest
{
    private readonly Mock<ILogger<OrderCreatedConsumer>> _loggerMock;
    private readonly Mock<IReplicateOrderUseCase> _replicateOrderUseCase;
    private readonly OrderCreatedConsumer _consumer;

    public OrderCreatedConsumerTest()
    {
        _loggerMock = new Mock<ILogger<OrderCreatedConsumer>>();
        _replicateOrderUseCase = new Mock<IReplicateOrderUseCase>();
        _consumer = new OrderCreatedConsumer(_loggerMock.Object, _replicateOrderUseCase.Object);
    }

    [Fact]
    public async Task Consume_ShouldLogInformationAndSave_WhenCalled()
    {
        // Arrange
        var orderCreated = new Fixture().Create<OrderCreated>();
        var contextMock = new Mock<ConsumeContext<OrderCreated>>();
        contextMock.Setup(c => c.Message).Returns(orderCreated);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.VerifyLog(logger => logger.LogInformation("Message processed: {Message}", orderCreated),
            LogLevel.Information, Times.Exactly(2));
        _replicateOrderUseCase.Verify(gateway => gateway.ExecuteAsync(It.IsAny<KitchenOrderDto>(), default), Times.Once);
    }
    
    [Fact]
    public async Task Consume_ShouldLogErrorAndThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var orderCreated = new Fixture().Create<OrderCreated>();
        var contextMock = new Mock<ConsumeContext<OrderCreated>>();
        contextMock.Setup(c => c.Message).Returns(orderCreated);
        _replicateOrderUseCase
            .Setup(gateway => gateway.ExecuteAsync(It.IsAny<KitchenOrderDto>(), default))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var act = async () => await _consumer.Consume(contextMock.Object);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Test exception");

        // Verify that the error was logged
        _loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), 
            "Failed to process order created message: {ErrorMessage}", 
            "Test exception"), LogLevel.Error, Times.Once());
    }
}