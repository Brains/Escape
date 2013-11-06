using System.Linq;
using Traffic.Actions;
using Traffic.Cars;

namespace Traffic.Drivers
{
    internal class Common : Driver
    {
        private float cruiseZone;

        //------------------------------------------------------------------
        public Common (Car car)
        {
            Car = car;

            Add (new Loop (Car.Accelerate));
            Add (new Loop (AvoidCollisions));

            //            cruiseZone = Car.Lenght * 4.0f;
        }

        //-----------------------------------------------------------------
        private void ChangeLane ()
        {
            ChangeLane (Lane.Random.Next (2) == 0 ? Car.Lane.Left : Car.Lane.Right);
        }
    }
}