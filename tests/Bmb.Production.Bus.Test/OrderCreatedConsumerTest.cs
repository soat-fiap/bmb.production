using AutoFixture;
using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;
using FluentAssertions;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Production.Bus.Test;

[TestSubject(typeof(OrderCreatedConsumer))]
public class OrderCreatedConsumerTest
{
    private readonly Mock<ILogger<OrderCreatedConsumer>> _loggerMock;
    private readonly Mock<IKitchenOrderRepository> _kitchenQueueGatewayMock;
    private readonly OrderCreatedConsumer _consumer;

    public OrderCreatedConsumerTest()
    {
        _loggerMock = new Mock<ILogger<OrderCreatedConsumer>>();
        _kitchenQueueGatewayMock = new Mock<IKitchenOrderRepository>();
        _consumer = new OrderCreatedConsumer(_loggerMock.Object, _kitchenQueueGatewayMock.Object);
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
        _kitchenQueueGatewayMock.Verify(gateway => gateway.SaveAsync(It.IsAny<KitchenOrderDto>(), default), Times.Once);
    }
    
    [Fact]
    public async Task Consume_ShouldLogErrorAndThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var orderCreated = new Fixture().Create<OrderCreated>();
        var contextMock = new Mock<ConsumeContext<OrderCreated>>();
        contextMock.Setup(c => c.Message).Returns(orderCreated);
        _kitchenQueueGatewayMock
            .Setup(gateway => gateway.SaveAsync(It.IsAny<KitchenOrderDto>(), default))
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