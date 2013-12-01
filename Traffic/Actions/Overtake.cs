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
    internal class Overtake : Sequence
    {
        private Driver driver;
        private Car target;

        //------------------------------------------------------------------
        public Overtake (Driver driver, Car target)
        {
            this.driver = driver;
            this.target = target;
            Name = "Overtake";
            Add (new Generic (Start) {Name = "Catch"});
        }

        //------------------------------------------------------------------
        public override Base.Action Copy()
        {
            return new Overtake (driver, target);
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

            // Change Lane if Target is visible
            const int visibility = 100;

            if (driver.Distance (target) < visibility)
                MatchLane();
        }

        //------------------------------------------------------------------
        private void Catch()
        {
            const int force = 5;

            if (driver.IsAhead (target))
                Accelerate (this, force);
            else
                driver.Brake (this, force);
        }

        //------------------------------------------------------------------
        private void MatchLane()
        {
            float lane = driver.Car.Lane.GlobalPosition.X;
            float targetLane = target.Lane.GlobalPosition.X;

            if (Math.Abs (lane - targetLane) < float.Epsilon)
                return;
            else if (lane < targetLane)
                ChangeLane (driver.Car.Lane.Right);
            else if (lane > targetLane)
                ChangeLane (driver.Car.Lane.Left);
        }

        //------------------------------------------------------------------
        private void ChangeLane (Lane lane)
        {
            if (!driver.TryChangeLane (this, lane, driver.GetChangeLanesDuration()))
                return;

            if (!driver.Car.Blinker.Visible)
                driver.Car.EnableBlinker (lane);
        }

        //------------------------------------------------------------------
        public void Accelerate (Composite action, int times)
        {
            if (driver.Car.Velocity < driver.Velocity)
                action.Add (new Repeated (driver.Car.Accelerate, times) {Name = "Accelerate"});
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
            new Text (actionsNames, driver.Car.GlobalPosition, Color.SteelBlue, true);
        }
    }
}