using System;
using System.Diagnostics;
using System.Linq;
using Engine.Actions;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Shrink : SequenceInitial
    {
        private readonly Driver driver;
        private Car closest; // Ahead

        //------------------------------------------------------------------
        public Shrink (Driver driver)
        {
            this.driver = driver;
            Initial = new Generic (AnalyzeDistance);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            FindClosestCar();

            base.Update (elapsed);

            Debug();
        }

        #region Analysis

        //------------------------------------------------------------------
        private void AnalyzeDistance()
        {
            float distance = driver.Distance (closest);

            // Define different dangerous zones
            float high = driver.SafeZone.HighDanger;
            float medium = driver.SafeZone.MediumDanger;
            float low = driver.SafeZone.LowDanger;

            // There is a free way
            if (distance > low)
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

            DrawZone (distance, high, medium, low);
        }

        //------------------------------------------------------------------
        private void FindClosestCar()
        {
            closest = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsCarAhead));
        }

        //------------------------------------------------------------------
        private Lane GetPrimaryLane()
        {
            if (driver.Primary == Driver.Direction.Left)
                return driver.Car.Lane.Left;
            else
                return driver.Car.Lane.Right;
        }

        //------------------------------------------------------------------
        private Lane GetSecondaryLane()
        {
            if (driver.Primary == Driver.Direction.Left)
                return driver.Car.Lane.Right;
            else
                return driver.Car.Lane.Left;
        }

        #endregion

        //------------------------------------------------------------------
        private void TryChangeLanes()
        {
            if (closest == null) return;

            if (driver.TryChangeLane (this, GetPrimaryLane(), driver.GetChangeLanesDuration()))
                return;

            if (driver.TryChangeLane (this, GetSecondaryLane(), driver.GetChangeLanesDuration()))
                return;

            // Brake if no free Lanes
            driver.Brake (this, 20);
        }

        //------------------------------------------------------------------
        private void EnableBlinker()
        {
//            if (driver.Car.IsBlinkerEnable()) return;

            Lane primary = GetPrimaryLane();
            Lane secondary = GetSecondaryLane();

            if (driver.CheckLane (primary))
                driver.Car.EnableBlinker (primary);
            else if (driver.CheckLane (secondary))
                driver.Car.EnableBlinker (secondary);
        }

        //------------------------------------------------------------------
        private void Debug()
        {
            // Draw SafeZone
//            var pos = driver.Car.Position;
//            new Line (pos, pos - new Vector2 (0, driver.SafeZone), Color.IndianRed);

            // Mark closest car
//            var pos = driver.Car.Position;
//            if (closest is Cars.Player)
//                new Line (pos, closest.Position);
        }

        //-----------------------------------------------------------------
        private void DrawZone (float distance, float high, float medium, float low)
        {
//            string text = "";
//
//            if (distance < high)
//                text = "High";
//            else if (distance < medium)
//                text = "Medium";
//            else if (distance < low)
//                text = "Low";
//
//            var shift = new Vector2 (30, 0);
//            if (driver.Primary == Driver.Direction.Left)
//                new Line (driver.Position, driver.Position - shift, Color.Orange);
//            else
//                new Line (driver.Position, driver.Position + shift, Color.Orange);

//            Console.WriteLine ("{0} : {1} {2} {3}", text, GetPrimaryLane(), driver.Car.Lane, GetSecondaryLane());
        }
    }
}