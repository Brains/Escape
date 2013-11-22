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
            Add (new Generic (Start) {Name = "AdjustSpeed"});
        }

        //------------------------------------------------------------------
        public override Base.Action Copy ()
        {
            return new Overtake (driver, target);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

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
                driver.TryChangeLane (driver.Car.Lane.Right, this, driver.GetChangeLanesDuration());
            else if (lane > targetLane)
                driver.TryChangeLane (driver.Car.Lane.Left, this, driver.GetChangeLanesDuration());
        }

        //------------------------------------------------------------------
        public void Accelerate (Composite action, int times)
        {
            if (driver.Car.Velocity < driver.Velocity)
                action.Add (new Repeated (driver.Car.Accelerate, times) { Name = "Accelerate" });
        }

        //------------------------------------------------------------------
        void Debug ()
        {
//            new Text (Actions.Count.ToString (), Vector2.One * 50, Color.MediumOrchid, true);
            //Console.WriteLine (Actions);
        }

    }
}