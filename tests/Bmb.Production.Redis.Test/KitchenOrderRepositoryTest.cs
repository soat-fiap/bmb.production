using System.Text.Json;
using AutoFixture;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using Bmb.Test.Common;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace Bmb.Production.Redis.Test;

[TestSubject(typeof(KitchenOrderRepository))]
public class KitchenOrderRepositoryTest
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ILogger<KitchenOrderRepository>> _mockLogger;
    private readonly KitchenOrderRepository _repository;
    private readonly Mock<IDatabase> _mockDatabase;

    public KitchenOrderRepositoryTest()
    {
        _mockDatabase = new Mock<IDatabase>();
        _mockLogger = new Mock<ILogger<KitchenOrderRepository>>();

        var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        mockConnectionMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _repository = new KitchenOrderRepository(_mockLogger.Object, mockConnectionMultiplexer.Object);
    }

    [Fact]
    public async Task SaveAsync_ShouldLogMessages_WhenOrderIsSaved()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();

        // Act
        await _repository.SaveAsync(order);

        // Assert
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), order.OrderId), LogLevel.Information,
            Times.Exactly(2));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders_WhenOrdersExist()
    {
        // Arrange
        var orders = new List<RedisValue> { "order1" };
        var expectedOrders = new List<KitchenOrderDto>
        {
            new(Guid.NewGuid(), "order1", [], KitchenOrderStatus.Received)
        };

        _mockDatabase
            .Setup(db => db.SetMembersAsync(KitchenOrderRepository.KdsOrderSet, It.IsAny<CommandFlags>()))
            .ReturnsAsync(expectedOrders.Select(i => (RedisValue)i.OrderId.ToString()).ToArray());

        _mockDatabase
            .Setup(db => db.HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(JsonSerializer.Serialize(expectedOrders.First())));

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(orders.Count);
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            LogLevel.Information, Times.Exactly(2));
    }

    [Fact]
    public async Task GetAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        _mockDatabase
            .Setup(db => db.HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(JsonSerializer.Serialize(order));

        // Act
        var result = await _repository.GetAsync(order.OrderId);

        // Assert
        result.Should().BeEquivalentTo(order);
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), order.OrderId), LogLevel.Information,
            Times.Exactly(2));
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockDatabase
            .Setup(db => db.HashGetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _repository.GetAsync(orderId);

        // Assert
        result.Should().BeNull();
        _mockLogger.VerifyLog(logger => logger.LogWarning(It.IsAny<string>(), orderId), LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task EnqueueOrderAsync_ShouldEnqueueOrder_WhenCalled()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        var mockTransaction = new Mock<ITransaction>();
        _mockDatabase.Setup(s => s.CreateTransaction(default)).Returns(mockTransaction.Object);

        // Act
        await _repository.EnqueueOrderAsync(order);

        // Assert
        mockTransaction.Verify(
            db => db.ListLeftPushAsync(It.IsAny<RedisKey>(), order.OrderId.ToString(), It.IsAny<When>(),
                It.IsAny<CommandFlags>()), Times.Once);
        mockTransaction.Verify(
            db => db.SetAddAsync(It.IsAny<RedisKey>(), order.OrderId.ToString(), CommandFlags.None), Times.Once());
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), order.OrderId), LogLevel.Information,
            Times.Exactly(2));
    }

    [Fact]
    public async Task GetNextOrderAsync_ShouldReturnNextOrder_WhenOrdersExist()
    {
        // Arrange
        var order = _fixture.Create<KitchenOrderDto>();
        _mockDatabase.Setup(db => db.ListRightPopAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(JsonSerializer.Serialize(order));

        // Act
        var result = await _repository.GetNextOrderAsync();

        // Assert
        result.Should().BeEquivalentTo(order);
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task GetNextOrderAsync_ShouldReturnNull_WhenNoOrdersExist()
    {
        // Arrange & Act
        var result = await _repository.GetNextOrderAsync();

        // Assert
        result.Should().BeNull();
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>()), LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenCalled()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var status = KitchenOrderStatus.Ready;

        // Act
        await _repository.UpdateStatusAsync(orderId, status);

        // Assert
        _mockDatabase.Verify(
            db => db.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()), Times.Once);
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), orderId, status),
            LogLevel.Information, Times.Exactly(2));
    }

    [Fact]
    public async Task DeleteOrderAsync_ShouldDeleteOrder_WhenCalled()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        await _repository.DeleteOrderAsync(orderId);

        // Assert
        _mockDatabase.Verify(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Exactly(2));
        _mockLogger.VerifyLog(logger => logger.LogInformation(It.IsAny<string>(), orderId), LogLevel.Information,
            Times.Exactly(2));
    }
}