using System.Linq;
using Microsoft.Xna.Framework;

namespace Traffic.Drivers
{
    internal class Player : Driver
    {

        //------------------------------------------------------------------
        public Player (Car car)
        {
            Car = car;
        }

        //------------------------------------------------------------------
        public override void Create ()
        {
        }

        //------------------------------------------------------------------
        public override void Update ()
        {
            var closestCar = FindClosestCar (Car.Lane.Cars);
            closestCar.Color = Color.GreenYellow;
        }
    }
}