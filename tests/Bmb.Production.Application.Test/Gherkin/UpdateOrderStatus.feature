Feature: Update Order status and publish Notification
  
  Scenario: Update an order status successfully
    Given an OrderId is received
    And the OrderId exists on the database
    When the UseCase is executed
    Then it should update the order status
    And it should publishes a OrderStatusUpdated event