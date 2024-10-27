Feature: Prepare order
  
  Scenario: Prepare an order successfully
    Given an OrderId is received
    When the OrderId exists on the database
    Then it should add the order to Received queue
    And it should log the operation details
    And it should publishes a OrderStatusUpdated event
    
  Scenario: Order doesn't exist
    Given an OrderId is received
    When the OrderId doesn't exist on the database
    Then it should log the operation details
    And return a null response