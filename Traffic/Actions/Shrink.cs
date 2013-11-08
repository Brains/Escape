using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Shrink : Loop
    {
        private readonly Driver driver;
        private Car closestCar;

        //------------------------------------------------------------------
        public Shrink (Driver driver)
        {
            this.driver = driver;
            Name = "Shrink";
            Initial = new Generic (DetectDanger) { Name = "DetectDanger" };
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            FindClosestCar ();

            base.Update (elapsed);

            Debug ();
        }

        //------------------------------------------------------------------
        private void DetectDanger ()
        {
            // ToDo: In statement below is window [200, 300] with constant speed
//            if (driver.Distance (closestCar) > 300)

            if (!IsThreatOfCollision ())
            {
                driver.Accelerate (this, 5);
                return;
            }

            if (driver.Distance (closestCar) < driver.DangerousZone / 1.5)
                driver.Brake (this, 50);
            else
                // Change Lane
                Add (new Conditional (IsThreatOfCollision, TryChangeLanes) {Name = "Try Change Lane"});
        }

        //------------------------------------------------------------------
        private bool IsThreatOfCollision ()
        {
            if (closestCar == null) return false;

            bool mySpeedLarger = driver.Car.Velocity > closestCar.Velocity;
            bool tooCloseDistance = driver.Distance (closestCar) < driver.DangerousZone;

            return tooCloseDistance && mySpeedLarger;
        }

        //------------------------------------------------------------------
        private void FindClosestCar ()
        {
            closestCar = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsAhead));
        }

        //------------------------------------------------------------------
        private void TryChangeLanes ()
        {
            // Change Lane on Left
            if (driver.TryChangeLane (driver.Car.Lane.Left, this)) return;

            if (driver.TryChangeLane (driver.Car.Lane.Right, this)) return;

            // Last chance to avoid collision if no free Lanes
            driver.Brake (this, 50);
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
            
        }

    }
}