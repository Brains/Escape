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

            // Set safely distance
            if (IsThreatOfCollision ())
                driver.Brake (this, 5);

            // Change Lane
            Add (new Conditional (IsThreatOfCollision, TryChangeLanes) { Name = "Try Change Lane" });

            // Last chance to avoid collision
            if (IsThreatOfCollision ())
                driver.Brake (this, 50);
        }

        //------------------------------------------------------------------
        private bool IsThreatOfCollision ()
        {
            Car closestCar = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsAhead));
            if (closestCar == null) return false;

            bool mySpeedLarger = driver.Car.Velocity > closestCar.Velocity;
            bool tooCloseDistance = driver.Distance (closestCar) < 200;

            return tooCloseDistance && mySpeedLarger;
        }

        //------------------------------------------------------------------
        private void TryChangeLanes ()
        {
            // Change Lane on Left
            if (driver.TryChangeLane (driver.Car.Lane.Left, this)) return;

            if (driver.TryChangeLane (driver.Car.Lane.Right, this)) return;

//            var sequence = new Sequence () { Name = "Change Lane" };
//            var sequence = new Sequence () { Name = "Blinker" };
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
            
        }

    }
}