using AutoFixture;
using Bmb.Domain.Core.Events.Notifications;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Bus.Consumers;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Production.Bus.Test.Consumers;

[TestSubject(typeof(OrderPaymentConfirmedConsumer))]
public class OrderPaymentConfirmedConsumerTest
{
    private readonly Mock<ILogger<OrderPaymentConfirmedConsumer>> _loggerMock;
    private readonly Mock<IReceiveOrderUseCase> _receiveOrderUseCaseMock;
    private readonly OrderPaymentConfirmedConsumer _consumer;

    public OrderPaymentConfirmedConsumerTest()
    {
        _loggerMock = new Mock<ILogger<OrderPaymentConfirmedConsumer>>();
        _receiveOrderUseCaseMock = new Mock<IReceiveOrderUseCase>();
        _consumer = new OrderPaymentConfirmedConsumer(_loggerMock.Object, _receiveOrderUseCaseMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldLogErrorAndThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var orderPaymentConfirmed = new Fixture().Create<OrderPaymentConfirmed>();
        var contextMock = new Mock<ConsumeContext<OrderPaymentConfirmed>>();
        contextMock.Setup(c => c.Message).Returns(orderPaymentConfirmed);
        _receiveOrderUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(It.IsAny<Guid>(), default))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var act = async () => await _consumer.Consume(contextMock.Object);

        // Assert
        using (new AssertionScope())
        {
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Test exception");

            _loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), 
                "Failed to process Payment confirmation message: {ErrorMessage}", 
                "Test exception"), LogLevel.Error, Times.Once());
        }
    }
}