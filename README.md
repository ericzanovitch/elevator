# elevator
Coderbyte Elevator Control Service

The elevator will go from the floor it is on to the floor it is called to.  

If it is called externally, the direction will determine whether or not the elevator will stop on the desired floor.  This means that when an elevator is called to go up, it will not stop while passing by going down and vice versa.  If a call is made for both directions, each direction represents a distinct call.

Only calls made from within the elevator will register on the display there.  Outside calls will be invisible to an inside rider.

When no requests are pending, the elevator will return to the first floor.

