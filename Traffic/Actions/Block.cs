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
        public Block (Driver driver, Car target)
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
            // Finish action if target is ahead
            if (driver.IsCarAhead (target)) return;

            // Match Lane
            bool visible = driver.Distance (target) < 300;
            bool overtake = Math.Abs (driver.Car.Velocity - target.Velocity) < 100;

            if (visible && overtake)
                MatchLane();
        }

        //------------------------------------------------------------------
        private void MatchLane()
        {
            var scaleBackup = driver.SafeZone.Scale;

            // Reduce Safe Zone to more agressive behaviour
            driver.SafeZone.Scale = 0.3f;

            // Try to Block Target
            int currentID = driver.Car.Lane.ID;
            int targetID = target.Lane.ID;

            Lane right = driver.Car.Lane.Right;
            Lane left = driver.Car.Lane.Left;

            const float delay = 0.3f;

            if (currentID == targetID)
                ;
            else
            {
                if (currentID < targetID)
                {
                    Add (new Sleep (delay));
                    driver.TryChangeLane (this, right, driver.GetChangeLanesDuration());
                    driver.Primary = Driver.Direction.Right;
                }
                else if (targetID < currentID)
                {
                    Add (new Sleep (delay));
                    driver.TryChangeLane (this, left, driver.GetChangeLanesDuration ());
                    driver.Primary = Driver.Direction.Left;
                }
            }

            // Restore Safe Zone
            driver.SafeZone.Scale = scaleBackup;
        }

        //------------------------------------------------------------------
        private void Debug()
        {
//            DrawActions();

//            new Text ("Block", driver.Car.Position, Color.DarkOrange, true);
        }
    }
}