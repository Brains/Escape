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
    internal class Shrink : SequenceInitial
    {
        private readonly Driver driver;
        private Car closest; // Ahead

        //------------------------------------------------------------------
        public Lane Desired { get; set; }

        //------------------------------------------------------------------
        public Shrink (Driver driver)
        {
            this.driver = driver;
            Desired = driver.Car.Lane.Left;
            Initial = new Generic (AnalyzeDistance);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            FindClosestCar ();

            base.Update (elapsed);

            Debug ();
        }

        #region Analysis

        //------------------------------------------------------------------
        private void AnalyzeDistance ()
        {
            float distance = driver.Distance (closest);
            float safe = driver.GetSafeZone ();
            
            // Define different dangerous zones
            float high = driver.GetSafeZone (0.33f);
            float medium = driver.GetSafeZone (0.66f);
            float low = driver.GetSafeZone (1.0f);

            // There is a free way
            if (distance > safe)
            {
                driver.Car.DisableBlinker();
                return;
            }

            // Keep Safe Distance
            if (distance < high)
                driver.Brake (this, 20);

            // Ahead Car approaches
            if (driver.Car.Velocity > closest.Velocity)
                AvoidCollision (distance, high, medium, low);
        }

        //-----------------------------------------------------------------
        private void AvoidCollision (float distance, float high, float medium, float low)
        {
            // If closest Car approaches too fast
            if (driver.Car.Velocity - closest.Velocity > 100)
                driver.Brake (this, 20);

            // High threat of the collision
            if (distance < high)
                driver.Brake (this, 50); // Strong braking

            // Bypass the closest Car
            else if (distance < medium)
                TryChangeLanes();

            // Enable Blinker
            else if (distance < low)
                EnableBlinker();
        }

        //------------------------------------------------------------------
        private void FindClosestCar ()
        {
            closest = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsAhead));
        }

        #endregion

        //------------------------------------------------------------------
        private void TryChangeLanes ()
        {
            if (closest == null) return;

            // Change Lane on Desired
            if (driver.TryChangeLane (this, Desired, driver.GetChangeLanesDuration ())) 
                return;

            // Change Lane on other Lane
            Lane left = driver.Car.Lane.Left;
            Lane right = driver.Car.Lane.Right;
            Lane other = (Desired == left) ? right : left;

            if (driver.TryChangeLane (this, other, driver.GetChangeLanesDuration ())) 
                return;

            // Brake if no free Lanes
            driver.Brake (this, 20);
        }

        //------------------------------------------------------------------
        private void EnableBlinker ()
        {
            if (driver.Car.Blinker.Visible) return;

            if (driver.CheckLane (driver.Car.Lane.Left))
                driver.Car.EnableBlinker (driver.Car.Lane.Left);
            else if (driver.CheckLane (driver.Car.Lane.Right))
                driver.Car.EnableBlinker (driver.Car.Lane.Right);
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
            // Draw SafeZone
//            var pos = driver.Car.Position;
//            new Line (pos, pos - new Vector2 (0, driver.SafeZone), Color.IndianRed);

            // Mark closest car
//            var pos = driver.Car.Position;
//            if (closest is Cars.Player)
//                new Line (pos, closest.Position);
        }
    }
}