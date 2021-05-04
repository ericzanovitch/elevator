using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elevator.Controllers
{
    [ApiController]
    [Route("car")]
    public class CarController : Controller
    {
        // GET: CarController
        public List<int> Index()
        {
            return Elevator.car.Floors;
        }

        // GET: CarController/Next
        [HttpGet("Next")]
        public int Next()
        {
            return Elevator.car.NextFloor();
        }

        // POST: CarController/FromCar
        [HttpPost("CallFromCar")]
        public bool CallFromCar([FromBody]CallParameters FloorRequest)
        {
            return Elevator.car.RequestServiceToFloor(FloorRequest.Floor);
        }

        // POST: CarController/CallFromFloor
        [HttpPost("CallFromFloor")]
        public bool CallFromFloor([FromBody] CallParameters FloorRequest)
        {
            return Elevator.car.RequestServiceFromFloor(FloorRequest.Floor, FloorRequest.Direction);
        }
    }
}
