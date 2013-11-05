using System.Linq;
using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Common : Driver
    {
        private float cruiseZone;

        //------------------------------------------------------------------
        public Common (Car car)
        {
            Car = car;
        }

        //------------------------------------------------------------------
        public override void Create ()
        {
            base.Create ();

            cruiseZone = Car.Lenght * 4.0f;

            Add (new Loop (Car.Accelerate));
            Add (new Loop (AvoidCollisions));
        }

        //-----------------------------------------------------------------
        private void ChangeLane ()
        {
            ChangeLane (Lane.Random.Next (2) == 0 ? Car.Lane.Left : Car.Lane.Right);
        }
    }
}