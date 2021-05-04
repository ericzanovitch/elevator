# elevator
Coderbyte Elevator Control Service

Rules
* The elevator will go from the floor it is on to the floor it is called to.  
* If it is called externally, the direction will determine whether or not the elevator will stop on the desired floor.  
  This means that when an elevator is called to go up, it will not stop while passing by going down and vice versa.  
  If a call is made for both directions, each direction represents a distinct call.
* Only calls made from within the elevator will register on the display there.  Outside calls will be invisible to an inside rider.
* When no requests are pending, the elevator will return to the first floor.

Calls
* GET http://localhost:8080/car will return all floors chosen from within the elevator
* GET http://localhost:8080/car/next will return the next floor to go to.  That floor will be removed from the queue.
* GET http://localhost:8080/car/status will return the current floor and direction.  The direction value is 1 for up, -1 for down.
* POST http://localhost:8080/car/callfromcar will add the requested floor to the inside queue.  
  The request body will contain the floor in a CallParameter object.  The direction value is not needed here.
* POST http://localhost:8080/car/callfromfloor will add the requested floor to the ouside queue.  
  The request body will contain the floor and direction in a CallParameter object.  The direction value is 1 for up, -1 for down.

* DELETE http://localhost:8080/car is a function used for testing and debugging.  It clears all queues and resets the direction and floor.

Execution
* It should be possible to execute the service by running "dotnet .\bin\Release\netcoreapp3.1\elevator.dll" from the root project folder.
* The number of floors can be changed by adding a "floors" value in the appsettings.json file.  If no value is specified there, it defaults to 10.

Testing
* There are two testing scripts in the test folder.  These use Curl to execute test calls against the service.
*   BasicTest.cmd runs through a simple sequence of calls that represent a basic level of functionality.
*   AdvancedTest.cmd runs through more challenging simulations by calling floors out of order in order to test priority handling.
* Each test returns the results given by the service and displays them with the expected results.