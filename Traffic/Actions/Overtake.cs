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
        private Shrink shrink;

        //------------------------------------------------------------------
        public Overtake (Driver driver, Car target, Shrink shrink)
        {
            this.driver = driver;
            this.target = target;
            this.shrink = shrink;
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
            Catch();

            // Match Lane
            const int visibility = 200;
            bool visible = driver.Distance (target) < visibility;
            bool overtake = Math.Abs (driver.Car.Velocity - target.Velocity) < 100;

            if (visible && overtake)
                MatchLane();
        }

        //------------------------------------------------------------------
        private void Catch()
        {
            const int force = 3;

            if (driver.IsAhead (target))
                Accelerate (this, force);
            else
                Brake (force);
        }

        //------------------------------------------------------------------
        private void MatchLane()
        {
            float current = driver.Car.Lane.Position.X;
            float targetLane = target.Lane.Position.X;

            if (Math.Abs (current - targetLane) < float.Epsilon)
                return;
            else if (current < targetLane)
                TryChangeLane (driver.Car.Lane.Right);
            else if (current > targetLane)
                TryChangeLane (driver.Car.Lane.Left);
        }

        //------------------------------------------------------------------
        private void TryChangeLane (Lane lane)
        {
            shrink.Desired = lane;

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
        }

        //------------------------------------------------------------------
        protected void DrawActions()
        {
            var actionsNames = Actions.Aggregate ("\n", (current, action) => current + (action + "\n"));
            new Text (actionsNames, driver.Car.Position, Color.SteelBlue, true);
        }
    }
}