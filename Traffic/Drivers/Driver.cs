using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Extensions;
using Tools.Markers;
using Traffic.Actions;
using Traffic.Actions.Base;
using Traffic.Cars;
using Action = Traffic.Actions.Base.Action;

namespace Traffic.Drivers
{
    public abstract class Driver
    {
        //------------------------------------------------------------------
        protected Loop Loop;
        protected Sequence Sequence;

        protected float CheckLaneSafeZoneLower;
        protected float CheckLaneSafeZoneUpper;
        protected float ChangeLaneSpeed;

        //------------------------------------------------------------------
        public Car Car { get; set; }
        public float Velocity { get; set; }

        //------------------------------------------------------------------
        protected Driver (Car car)
        {
            Loop = new Loop ();
            Sequence = new Sequence();
            AddInLoop (Sequence);

            Car = car;

            ChangeLaneSpeed = 1;
        }

        //-----------------------------------------------------------------
        public virtual void Setup ()
        {
            CheckLaneSafeZoneLower = GetSafeZone (0.5f);
            CheckLaneSafeZoneUpper = GetSafeZone (1.5f);
        }

        #region Actions

        //-----------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            Loop.Update (elapsed);

            Debug ();
        }

        //------------------------------------------------------------------
        public void AddInLoop (Action action)
        {
            Loop.Add (action);
        }

        //------------------------------------------------------------------
        public void AddInSequnce (Action action)
        {
            Sequence.Add (action);
        }

        //------------------------------------------------------------------
//        private bool IsInLoop (Sequence newAction)
//        {
//            return Loop.Actions.Any (action => action.GetType () == newAction.GetType ());
//        }

        #endregion

        #region Sensor Analysis

        //------------------------------------------------------------------
        public float Distance (Car car)
        {
            if (car == null) return float.MaxValue;
            if (car == Car) return float.MaxValue;
            if (!car.IsIntersectActive()) return float.MaxValue;

            var distance = Car.Position - car.Position;

            return Math.Abs (distance.Y);
        }

        //------------------------------------------------------------------
        public Car FindClosestCar (IEnumerable <Car> cars)
        {
            return cars.MinBy (Distance);
        }

        //------------------------------------------------------------------
        protected float GetMinimumDistance (IEnumerable <Car> cars)
        {
            if (!cars.Any ()) return float.MaxValue;

            return cars.Min <Car, float> (Distance);
        }

        //------------------------------------------------------------------
        public bool IsAhead (Car car)
        {
            return car.Position.Y < Car.Position.Y;
        }

        //------------------------------------------------------------------
        public bool  CheckLane (Lane lane)
        {
            if (lane == null) return false;
            
            var closest = FindClosestCar (lane.Cars);
            if (closest == null) return true;
            float distance = Distance (closest);

            // If closest Car is outside the special zone
            if (distance < CheckLaneSafeZoneLower)
                return false;
            if (distance > CheckLaneSafeZoneUpper)
                return true;

            // Analyze Velocity
            if (IsAhead (closest) && closest.Velocity < Car.Velocity)
                return false;
            if (!IsAhead (closest) && closest.Velocity > Car.Velocity)
                return false;

            return true;
        }

        //-----------------------------------------------------------------
        public float GetChangeLanesDuration ()
        {
            var duration = 200 / Car.Velocity;
            duration /= ChangeLaneSpeed;
            const int limit = 4; // In seconds

            return duration < limit ? duration : limit;
        }

        //------------------------------------------------------------------
        public float GetSafeZone (float factor = 1.0f)
        {
            const float scale = 0.7f;

            return Car.Lenght + Car.Velocity * factor * scale;
        }

        #endregion

        #region Car Controll

        //------------------------------------------------------------------
        public void Brake (Composite action, int times = 1)
        {
            action.Add (new Repeated (Car.Brake, times) {Name = "Brake"});
        }

        //------------------------------------------------------------------
        public void Accelerate (Composite action, int times = 1)
        {
            if (Car.Velocity < Car.Lane.Velocity)
                action.Add (new Repeated (Car.Accelerate, times) {Name = "Accelerate"});
        }

        //------------------------------------------------------------------
        public bool TryChangeLane (Sequence action, Lane lane, float duration)
        {
            // Prevent changing on incorrect Lanes
            if (lane == null) return false;
            if (lane != Car.Lane.Left && lane != Car.Lane.Right) return false;

            // Check free space on Lane
            if (!CheckLane (lane)) return false;

            // Change Lane
            ChangeLane (lane, action, duration);

            return true;
        }

        //------------------------------------------------------------------
        public void ChangeLane (Lane lane, Sequence action, float duration)
        {
            if (lane == null) return;

            // No Lane changing when car doesn't move
            if (Car.Velocity < 10) return;

            // Rotate
            Action <float> rotate = share => Car.Angle += share;
            float finalAngle = MathHelper.ToRadians ((lane.Position.X < Car.Position.X) ? -10 : 10);
            action.Add (new Controller (rotate, finalAngle, duration * 0.3f));

            // Moving
            Action <Vector2> move = shift => Car.LocalPosition += shift;
            var diapason = new Vector2 (lane.Position.X - Car.Position.X, 0);
            action.Add (new Controller (move, diapason, duration * 0.4f));

            // Inverse rotating
            var inverseRotating = new Controller (rotate, -finalAngle, duration * 0.3f);
            action.Add (inverseRotating);

            // Add to new Lane
            action.Add (new Generic (() => lane.Add (Car)));
        }

        #endregion

        #region Debug

        //-----------------------------------------------------------------
        private void Debug ()
        {
//            DrawSafeZone ();
//            DrawActions ();
//            DrawCheckLane (Car.Lane);
        }

        //------------------------------------------------------------------
        private void DrawSafeZone()
        {
            var pos = Car.Position;
            new Line (pos, pos - new Vector2 (0, GetSafeZone(1.0f)), Color.IndianRed);
        }

        //------------------------------------------------------------------
        protected void DrawActions ()
        {
            var actionsNames = Loop.Actions.Aggregate ("\n", (current, action) => current + (action + "\n"));
            new Text (actionsNames, Car.Position, Color.SteelBlue, true);
        }

        //------------------------------------------------------------------
        protected void WriteActions ()
        {
            var actionsNames = Loop.Actions.Aggregate ("", (current, action) => current + (action + "\n"));
            Console.WriteLine (actionsNames);
        }

        //------------------------------------------------------------------
        private void DrawCheckLane (Lane lane)
        {
            if (lane == null) return;

            var pos = lane.Position;
            pos.Y = Car.Position.Y;

            new Line (pos, pos - new Vector2 (0, GetSafeZone () / 2), Color.IndianRed);
            new Line (pos, pos + new Vector2 (0, GetSafeZone () * 1.5f), Color.Orange);
        }

        #endregion

    }
}