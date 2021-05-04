using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elevator
{
    public class CallParameters
    {
        public int Floor { get; set; }
        public ElevatorCar.Direction Direction { get; set; }

        public CallParameters()
        {
            Direction = ElevatorCar.Direction.Up;
        }
    }
}
