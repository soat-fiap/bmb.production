using AutoFixture;
using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Production.Bus.Test;

[TestSubject(typeof(OrderCreatedConsumer))]
public class OrderCreatedConsumerTest
{
    private readonly Mock<ILogger<OrderCreatedConsumer>> _loggerMock;
    private readonly Mock<IKitchenQueueGateway> _kitchenQueueGatewayMock;
    private readonly OrderCreatedConsumer _consumer;

    public OrderCreatedConsumerTest()
    {
        _loggerMock = new Mock<ILogger<OrderCreatedConsumer>>();
        _kitchenQueueGatewayMock = new Mock<IKitchenQueueGateway>();
        _consumer = new OrderCreatedConsumer(_loggerMock.Object, _kitchenQueueGatewayMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldLogInformationAndSaveCopy_WhenCalled()
    {
        // Arrange
        var orderCreated = new Fixture().Create<OrderCreated>();
        var contextMock = new Mock<ConsumeContext<OrderCreated>>();
        contextMock.Setup(c => c.Message).Returns(orderCreated);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.VerifyLog(logger => logger.LogInformation("Message processed: {Message}", orderCreated),
            LogLevel.Information, Times.Once());
        _kitchenQueueGatewayMock.Verify(gateway => gateway.SaveAsync(It.IsAny<KitchenOrderDto>(), default), Times.Once);
    }
}