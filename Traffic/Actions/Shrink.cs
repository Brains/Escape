using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Shrink : Sequence
    {
        private readonly Driver driver;
        private Car closestCar;

        //------------------------------------------------------------------
        public Shrink (Driver driver)
        {
            this.driver = driver;
            Name = "Shrink";
            
            Add (new Generic (DetectDanger) {Name = "DetectDanger"});
        }

        //------------------------------------------------------------------
        public override Base.Action Copy ()
        {
            return new Shrink (driver);
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
            if (!IsThreatOfCollision ())
            {
                driver.Accelerate (this, 5);
                return;
            }

            // There is a Threat of Collision
            if (driver.Distance (closestCar) > driver.DangerousZone / 1.8f)
                // Change Lane if we have enough space
                Add (new Conditional (IsThreatOfCollision, TryChangeLanes) {Name = "Try Change Lane"});
            else 
                // Or try to strong Brake to avoid a collision
                driver.Brake (this, 50);
        }

        //------------------------------------------------------------------
        private bool IsThreatOfCollision ()
        {
            if (closestCar == null) return false;

            if (driver.Distance (closestCar) < driver.DangerousZone / 2.0f)
                return true;

            bool tooCloseDistance = driver.Distance (closestCar) < driver.DangerousZone;
            bool mySpeedLarger = driver.Car.Velocity > closestCar.Velocity;

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
            if (driver.TryChangeLane (driver.Car.Lane.Left, this, CalculateChangeLanesDuration ())) return;

            if (driver.TryChangeLane (driver.Car.Lane.Right, this, CalculateChangeLanesDuration ())) return;

            // Last chance to avoid collision if no free Lanes
            driver.Brake (this, 50);
        }

        //------------------------------------------------------------------
        private float CalculateChangeLanesDuration ()
        {
            float normal = driver.GetChangeLanesDuration ();
            float emergency = driver.GetChangeLanesDuration () / 2.0f;
            float velocityDifference = driver.Car.Velocity - closestCar.Velocity;

            if (closestCar == null) return normal;
            if (velocityDifference < float.Epsilon) return normal;

            float distanceToCollision = driver.Distance (closestCar) - (driver.Car.Lenght / 2.0f + closestCar.Lenght / 2.0f);
            bool distanceIsCritical = distanceToCollision < driver.DangerousZone / 3.0f;

            if (velocityDifference > 100 || distanceIsCritical)
                return emergency;

            return normal;
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
            // Draw DangerousZone
//            var pos = driver.Car.GlobalPosition;
//            new Line (pos, pos - new Vector2 (0, driver.DangerousZone), Color.IndianRed);

            // Mark closest car
//            var pos = driver.Car.GlobalPosition;
//            if (closestCar is Cars.Player)
//                new Line (pos, closestCar.GlobalPosition);
        }
    }
}