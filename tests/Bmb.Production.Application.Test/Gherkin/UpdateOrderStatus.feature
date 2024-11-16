Feature: Update Order status and publish Notification

  Scenario: Update an order status successfully
    Given an OrderId is received
    And the order status can be updated
    And the OrderId exists on the database
    And the event dispatcher is available
    When the UseCase is executed
    And it should publishes a OrderStatusUpdated event