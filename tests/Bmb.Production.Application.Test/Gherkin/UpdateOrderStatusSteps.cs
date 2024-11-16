using AutoFixture;
using Bmb.Domain.Core.Events;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using Moq;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;
using OrderStatusChanged = Bmb.Domain.Core.Events.Notifications.OrderStatusChanged;

namespace Bmb.Production.Application.Test.Gherkin;

[FeatureFile("./Gherkin/UpdateOrderStatus.feature")]
public class UpdateOrderStatusSteps : Feature
{
    private readonly UpdateOrderStatusUseCase _useCase;
    private readonly Mock<IKitchenOrderRepository> _mockKitchenOrderRepository;
    private readonly Mock<IDispatcher> _mockDispatcher;
    private Guid _orderId;

    public UpdateOrderStatusSteps()
    {
        _mockKitchenOrderRepository = new Mock<IKitchenOrderRepository>();
        _mockDispatcher = new Mock<IDispatcher>();
        _useCase = new UpdateOrderStatusUseCase(_mockKitchenOrderRepository.Object, _mockDispatcher.Object);
    }

    [Given("an OrderId is received")]
    public void GivenAnOrderIdIsReceived()
    {
        _orderId = Guid.NewGuid();
    }

    [And("the OrderId exists on the database")]
    public void WhenTheOrderIdExistsOnTheDatabase()
    {
        var order = new Fixture().Build<KitchenOrderDto>()
            .With(o => o.OrderId, _orderId)
            .Create();
        _mockKitchenOrderRepository.Setup(r => r.GetAsync(_orderId, default))
            .ReturnsAsync(order)
            .Verifiable();
    }

    [When("the UseCase is executed")]
    public Task Execute_UseCase()
    {
        return _useCase.ExecuteAsync(_orderId, KitchenOrderStatus.Preparing,default);
    }
    
    [Then("it should update the order status")]
    public void Update_OrderStatus()
    {
        _mockKitchenOrderRepository.Setup(r => r.UpdateStatusAsync(_orderId, KitchenOrderStatus.Preparing, default))
            .Returns(Task.CompletedTask)
            .Verifiable();
    }

    [And("it should publishes a OrderStatusUpdated event")]
    public void Publishes_OrderStatusUpdated_Event()
    {
        _mockDispatcher.Setup(d => d.PublishAsync(new OrderStatusChanged(_orderId, OrderStatus.InPreparation), default))
            .Returns(Task.CompletedTask)
            .Verifiable();
    }
}