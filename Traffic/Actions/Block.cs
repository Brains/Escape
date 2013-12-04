using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Block : SequenceInitial
    {
        private readonly Driver driver;
        private readonly Car target;

        //------------------------------------------------------------------
        public Block (Driver driver, Car target/*, Shrink shrink*/)
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
            //Car closest = driver.FindClosestCar ()

            // Finish action if target is ahead
            if (driver.IsCarAhead (target)) return;
            
            // Match Lane
            bool visible = driver.Distance (target) < 600;
//            bool overtake = Math.Abs (driver.Car.Velocity - target.Velocity) < 100;

            if (visible)
                MatchLane();
        }

        //------------------------------------------------------------------
        private void Catch()
        {
            const int force = 3;

            if (driver.IsCarAhead (target))
                Accelerate (this, force);
            else
                Brake (force);
        }

        //------------------------------------------------------------------
        private void MatchLane()
        {
            // Reduce Safe Zone to more agressive behaviour
            var scaleBackup = driver.SafeZone.Scale;
            driver.SafeZone.Scale = 0;

            // Try to Block Target
            int currentID = driver.Car.Lane.ID;
            int targetID = this.target.Lane.ID;

            Lane right = driver.Car.Lane.Right;
            Lane left = driver.Car.Lane.Left;

            if (currentID == targetID)
                ; // Don't return because we have to restore Scale Backup in the end
            else if (currentID < targetID)
            {
                driver.TryChangeLane (this, right, driver.GetChangeLanesDuration ());
                driver.SetLanesPriority (right, left);
            }
            else if (currentID > targetID)
            {
                driver.TryChangeLane (this, left, driver.GetChangeLanesDuration ());
                driver.SetLanesPriority (left, right);
            }

            // Restore Safe Zone
            driver.SafeZone.Scale = scaleBackup;
        }

        //------------------------------------------------------------------
        private void TryChangeLane (Lane lane)
        {
            driver.TryChangeLane (this, lane, driver.GetChangeLanesDuration());
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
                driver.Car.Brake ();
        }

        //------------------------------------------------------------------
        private void Debug()
        {
//            DrawActions();

            new Text ("Block", driver.Car.Position, Color.DarkOrange, true);

        }

        //------------------------------------------------------------------
        protected void DrawActions()
        {
            var actionsNames = Actions.Aggregate ("\n", (current, action) => current + (action + "\n"));
            new Text (actionsNames, driver.Car.Position, Color.SteelBlue, true);
        }
    }
}