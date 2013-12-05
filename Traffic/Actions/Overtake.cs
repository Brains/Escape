using System;
using System.Linq;
using System.Reflection;
using Android.OS;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Overtake : SequenceInitial
    {
        private Driver driver;
        private Car target;

        //------------------------------------------------------------------
        public Overtake (Driver driver, Car target)
        {
            this.driver = driver;
            this.target = target;
            Initial = new Generic (Start);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            Debug();
        }

        //------------------------------------------------------------------
        private void Start()
        {
            SetSafeZone();

            Catch();

            // Match Lane
            bool visible = driver.Distance (target) < 200;
            bool overtake = Math.Abs (driver.Car.Velocity - target.Velocity) < 100;

            if (visible && overtake)
                MatchLane();
        }

        //------------------------------------------------------------------
        private void SetSafeZone()
        {
            Car car = driver.Car;
            Car closest = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsCarAhead));
            if (closest == null) return;

            float approachSpeed = car.Velocity - closest.Velocity;
            float distance = driver.Distance (closest);

            driver.SafeZone.Scale = approachSpeed / 100;// * distance / 100;

            if (driver.SafeZone.Scale > 1.0f)
                driver.SafeZone.Scale = 1.0f;
            else if (driver.SafeZone.Scale < 0.1f)
                driver.SafeZone.Scale = 0.1f;
        }

        //------------------------------------------------------------------
        private void Catch()
        {
            const int force = 3;

            if (IsCarAhead (target))
                Accelerate (this, force);
            else
                Brake (force);
        }

        //------------------------------------------------------------------
        private bool IsCarAhead(Car target)
        {
            return driver.Car.Position.Y + 200 > target.Position.Y;
        }

        //------------------------------------------------------------------
        private void MatchLane()
        {
            int currentID = driver.Car.Lane.ID;
            int targetID = this.target.Lane.ID;

            Lane right = driver.Car.Lane.Right;
            Lane left = driver.Car.Lane.Left;

            if (currentID == targetID)
                ;
            else if (currentID < targetID)
            {
                driver.TryChangeLane (this, right, driver.GetChangeLanesDuration());
                driver.Primary = Driver.Direction.Right;

            }
            else if (currentID > targetID)
            {
                driver.TryChangeLane (this, left, driver.GetChangeLanesDuration());
                driver.Primary = Driver.Direction.Left;
            }
        }

        //------------------------------------------------------------------
        public void Accelerate (Composite action, int times = 1)
        {
            if (driver.Car.Velocity >= driver.Velocity) return;

            foreach (var i in Enumerable.Range (0, times))
                driver.Car.Accelerate();
        }

        //------------------------------------------------------------------
        private void Brake (int times)
        {
            foreach (var i in Enumerable.Range (0, times))
                driver.Car.Brake();
        }

        //------------------------------------------------------------------
        private void Debug()
        {
//            DrawActions();
        }

        //------------------------------------------------------------------
        protected void DrawActions()
        {
            var actionsNames = Actions.Aggregate ("\n", (current, action) => current + (action + "\n"));
            new Text (actionsNames, driver.Car.Position, Color.SteelBlue, true);
        }
    }
}