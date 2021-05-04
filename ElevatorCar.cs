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
        private List<int> floors = new List<int>();
        private List<int> shadowFloors = new List<int>();
        private Direction direction;

        public List<int> Floors { get { floors.Sort(); return floors; } }

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
            if (floors.Exists(value => value == floor))
                return false;
            floors.Add(floor);
            return true;
        }

        // Request a floor from an external panel.  Returns true if this is a new floor, false if the floor was already requested.
        // Requests with this function are directional and do not light up the car's internal panel
        public bool RequestServiceFromFloor(int floor, Direction serviceDirection)
        {
            if (serviceDirection == Direction.Down)
                floor = -floor;
            if (!shadowFloors.Contains(floor))
            {
                shadowFloors.Add(floor);
                return true;
            }
            return false;
        }

        public int NextFloor(int iteration = 0)
        {
            int newFloor = GetShadowFloor();
            if (floors.Count > 0)
            {
                if (direction == Direction.Down)
                    floors.Sort(delegate (int x, int y) { return x.CompareTo(y) * -1; });
                else
                    floors.Sort();

                try
                {
                    // This needs to be in a try/catch block since it will error if nothing is found.
                    int newMainFloor = floors.Where(n => n > currentFloor).First();
                    if (newMainFloor <= newFloor || newFloor == 0)
                    {
                        newFloor = newMainFloor;
                        floors.Remove(newFloor);
                    }
                    CancelShadowFloor(newFloor * (int)direction);
                    return newFloor;
                }
                catch 
                { 
                }
            }

            if (newFloor > 0)
            {
                CancelShadowFloor(newFloor);
                return newFloor;
            }

            // If all external floors have been serviced, switch directions
            direction = (Direction)(-(int)direction);
            currentFloor = (direction == Direction.Up) ? 1 : topFloor;

            // It will take one complete scan in each direction to ensure that there are no more requests.
            // When no more floors are available, leave the elevator on the first floor going up.
            iteration ++;
            return (iteration > 2 && direction == Direction.Up) ? 0 : NextFloor(iteration);
        }

        // Find the first shadow floor from the current floor with respect to the current direction
        private int GetShadowFloor()
        {
            if (shadowFloors.Count == 0)
                return 0;

            if (direction == Direction.Down)
                shadowFloors.Sort(delegate (int x, int y) { return x.CompareTo(y) * -1; });
            else
                shadowFloors.Sort();

            try
            {
                // This needs to be in a try/catch block since it will error if nothing is found.
                return ((direction == Direction.Up) ? shadowFloors.Where(n => n > currentFloor) : shadowFloors.Where(n => n < -currentFloor)).First();
            }
            catch
            {
                return 0;
            }

        }

        // Cancel a shadow floor with respect to direction
        private void CancelShadowFloor(int floor)
        {
            if (direction == Direction.Down)
                floor = -floor;
            shadowFloors.Remove(floor);
        }
    }
}
