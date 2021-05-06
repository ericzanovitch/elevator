using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Elevator
{
    public class ElevatorCar
    {
        public enum Direction
        {
            Up = 1,
            Down = -1
        }
        private int topFloor;
        private int currentFloor = 1;
        private List<int> insideFloors = new List<int>();
        private List<int> outsideFloors = new List<int>();
        private Direction direction;

        public List<int> Floors { get { insideFloors.Sort(); return insideFloors; } }
        public CallParameters Status { get { return new CallParameters() { Floor = currentFloor, Direction = direction }; } }

        public ElevatorCar(int top)
        {
            Debug.Assert(top > 1);
            topFloor = top;
            direction = Direction.Up;
        }

        // Request a floor.  Returns true if this is a new floor, false if the floor was already requested.
        // Requests with this function would light up on the internal display.
        public bool RequestServiceToFloor(int floor)
        {
            if (!FloorIsValid(floor))
                return false;
            lock (insideFloors)
            {
                if (insideFloors.Exists(value => value == floor))
                    return false;
                insideFloors.Add(floor);
                return true;
            }
        }

        // Request a floor from an external panel.  Returns true if this is a new floor, false if the floor was already requested.
        // Requests with this function are directional and do not light up the car's internal panel
        public bool RequestServiceFromFloor(int floor, Direction serviceDirection)
        {
            if (!FloorIsValid(floor))
                return false;
            lock(outsideFloors)
            {
                if (serviceDirection == Direction.Down)
                    floor = -floor;
                if (!outsideFloors.Contains(floor))
                {
                    outsideFloors.Add(floor);
                    outsideFloors.Sort(); // Since these floors are only serviced while going down, they only need to be sorted in descending order
                    return true;
                }
                return false;
            }
        }

        public int NextFloor(int iteration = 0)
        {
            lock(insideFloors)
            { 
                lock(outsideFloors)                
                {
                    int newFloor = GetOutsideFloor();
                    if (insideFloors.Count > 0)
                    {
                        try
                        {
                            // This needs to be in a try/catch block since it will error if nothing is found.
                            int newMainFloor;
                            if (direction == Direction.Down)
                            {
                                insideFloors.Sort(delegate (int x, int y) { return x.CompareTo(y) * -1; });
                                newMainFloor = insideFloors.Where(n => n < currentFloor).First();
                            }
                            else
                            {
                                insideFloors.Sort();
                                newMainFloor = insideFloors.Where(n => n > currentFloor).First();
                            }

                            // Remove directionality here since this won't be executed if no internal request is found
                            newFloor = (SameDirection(newFloor)) ? Math.Abs(newFloor) : 0;
                            if ((direction == Direction.Up && newMainFloor <= newFloor) || (direction == Direction.Down && newMainFloor >= newFloor) || newFloor == 0)
                            {
                                newFloor = newMainFloor;
                                insideFloors.Remove(newFloor);
                            }
                            CancelOutsideFloor(newFloor);
                            return (currentFloor = newFloor);
                        }
                        catch
                        {
                        }
                    }

                    if (newFloor != 0)
                    {
                        newFloor = Math.Abs(newFloor);
                        CancelOutsideFloor(newFloor);
                        return (currentFloor = newFloor);
                    }

                    // If all external floors have been serviced, switch directions
                    direction = (Direction)(-(int)direction);
                    currentFloor = (direction == Direction.Up) ? 1 : topFloor;

                    // It will take one complete scan in each direction to ensure that there are no more requests.
                    // When no more floors are available, leave the elevator on the first floor going up.
                    iteration++;
                    return (iteration > 2 && direction == Direction.Up) ? 0 : NextFloor(iteration);

                }
            }
        }

        // Delete function for testing purposes
        public bool ClearAll()
        {
            lock(insideFloors)
            {
                lock(outsideFloors)
                {
                    bool anyCleared = insideFloors.Count > 0 || outsideFloors.Count > 0;
                    insideFloors.Clear();
                    outsideFloors.Clear();
                    currentFloor = 1;
                    direction = Direction.Up;
                    return anyCleared;
                }
            }
        }

        // Find the first external floor from the current floor with respect to the current direction
        private int GetOutsideFloor()
        {
            if (outsideFloors.Count == 0)
                return 0;

            try
            {
                // This needs to be in a try/catch block since it will error if nothing is found.
                return ((direction == Direction.Up) ? outsideFloors.Where(n => n > currentFloor) : outsideFloors.Where(n => n > -currentFloor && n < 0)).First();
            }
            catch
            {
                return 0;
            }

        }

        // Cancel a external floor with respect to direction
        private void CancelOutsideFloor(int floor)
        {
            if (direction == Direction.Down)
                floor = -floor;
            outsideFloors.Remove(floor);
        }

        // Test the direction of an outside floor request.  True if compatible with the current direction
        private bool SameDirection(int floor)
        {
            return (Math.Abs(floor) * (int)direction == floor);
        }

        // Make sure the floor is within the range of allowed floors
        private bool FloorIsValid(int floor)
        {
            return (floor >= 1 && floor <= topFloor);
        }
    }
}
