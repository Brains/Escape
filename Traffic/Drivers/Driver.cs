using System.Linq;
using Microsoft.Xna.Framework;
using Tools;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Driver
    {
        private Car car;

        //------------------------------------------------------------------
        public Driver (Car car)
        {
            this.car = car;
        }

        //------------------------------------------------------------------
        public void Update ()
        {
            var closestCar = GetClosestCarAhead ();

            if (closestCar == null) return;

            float distance = Distance (closestCar);
//            closestCar.Color = Color.YellowGreen;

            float dangerousZone = (car.Height + closestCar.Height) / 1.0f;
            if (distance < dangerousZone)
            {
                car.Color = Color.Maroon;

                AvoidCollisions ();
            }
        }

        //------------------------------------------------------------------
        private Car GetClosestCarAhead ()
        {
            var aheadCars = car.Lane.Cars.Where (enemy => enemy.Position.Y < car.Position.Y);
            
            Car closestCar = aheadCars.MinBy <Car, float> (Distance);

            return closestCar;
        }

        //------------------------------------------------------------------
        private float Distance (Car enemy)
        {
            // Don't react with myself
            if (enemy == car) return float.MaxValue;

            var distance = car.Position - enemy.Position;

            return distance.Y;
        }

        //------------------------------------------------------------------
        private void AvoidCollisions ()
        {

            if (car.CheckLane (car.Lane.Left))
            {
                car.ChangeLane (car.Lane.Left);
                return;
            }

            if (car.CheckLane (car.Lane.Right))
            {
                car.ChangeLane (car.Lane.Right);
                return;
            }

            car.Brake ();
        }
    }
}