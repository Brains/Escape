using System;
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
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            // Loop sequnce
            if (Finished)
            {
                Add (new Generic (Start) { Name = "AdjustSpeed" });
                Finished = false;
            }

            Debug ();
        }

        //------------------------------------------------------------------
        private void Start ()
        {
            AdjustSpeed ();
            MatchLane ();
        }

        //------------------------------------------------------------------
        private void AdjustSpeed ()
        {
            if (target.GlobalPosition.Y > driver.Car.GlobalPosition.Y)
                driver.Brake (this, 5);
            else
                Accelerate (this, 5);
        }

        //------------------------------------------------------------------
        private void MatchLane ()
        {
            float lane = driver.Car.Lane.GlobalPosition.X;
            float targetLane = target.Lane.GlobalPosition.X;
            
            if (Math.Abs (lane - targetLane) < float.Epsilon)
                return;
            else if (lane < targetLane)
                driver.TryChangeLane (driver.Car.Lane.Right, this);
            else if (lane > targetLane)
                driver.TryChangeLane (driver.Car.Lane.Left, this);
        }

        //------------------------------------------------------------------
        public void Accelerate (Composite action, int times)
        {
            if (driver.Car.Velocity < 500)
                action.Add (new Repeated (driver.Car.Accelerate, times) { Name = "Accelerate" });
        }

        //------------------------------------------------------------------
        void Debug ()
        {
            new Text (driver.Distance (target).ToString (), target.GlobalPosition, Color.MediumOrchid, true);
        }

    }
}