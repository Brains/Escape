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
    public abstract class Driver : Object
    {
        public enum Direction
        {
            Left,
            Right
        }

        //------------------------------------------------------------------
        // Actions
        protected Loop Loop;
        protected Sequence Sequence;

        // Parameters
        public float ChangeLaneSpeed { get; set; }
        public Direction Primary { get; set; }

        //------------------------------------------------------------------
        public Car Car { get; set; }
        public float Velocity { get; set; }

        public SafeZone SafeZone { get; private set; }

        //------------------------------------------------------------------
        protected Driver (Car car) : base (car)
        {
            Loop = new Loop();
            Sequence = new Sequence();
            AddInLoop (Sequence);

            Car = car;

            Velocity = Car.Lane.Velocity;
            ChangeLaneSpeed = 1;
            SafeZone = new SafeZone (this, 1);
            Primary = Direction.Left;
        }

        #region Actions

        //-----------------------------------------------------------------
        public override void Update (float elapsed)
        {
            Loop.Update (elapsed);

            Debug();
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
        public bool IsInLoop (Sequence newAction)
        {
            return Loop.Actions.Any (action => action.GetType() == newAction.GetType());
        }

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
            if (!cars.Any()) return float.MaxValue;

            return cars.Min <Car, float> (Distance);
        }

        //------------------------------------------------------------------
        public bool IsCarAhead (Car car)
        {
            return car.Position.Y < Car.Position.Y;
        }

        //------------------------------------------------------------------
        public bool CheckLane (Lane lane)
        {
            if (lane == null) return false;

            var closestAhead = FindClosestCar (lane.Cars.Where (IsCarAhead));
            var closestBehind = FindClosestCar (lane.Cars.Where (car => !IsCarAhead (car)));
            
            return CheckCar (closestAhead) && CheckCar (closestBehind);
        }

        //-----------------------------------------------------------------
        private bool CheckCar (Car car)
        {
            if (car == null) return true;

            float distance = Distance (car);

            // If Car is too close
            if (distance < SafeZone.HighDanger)
                return false;
            // If Car is far enough it's Ok
            if (distance > SafeZone.LowDanger)
                return true;

            // Analyze Velocity
            if (IsCarAhead (car) && car.Velocity < Car.Velocity)
                return false;
            if (!IsCarAhead (car) && car.Velocity > Car.Velocity)
                return false;

            // Ok. The Car isn't dangerous
            return true;
        }

        //-----------------------------------------------------------------
        public float GetChangeLanesDuration()
        {
            var duration = 200 / Car.Velocity;
            duration /= ChangeLaneSpeed;
            const int limit = 4; // In seconds

            return duration < limit ? duration : limit;
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
            if (Car.Velocity < Car.Driver.Velocity)
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
            ChangeLane (action, lane, duration);

            return true;
        }

        //------------------------------------------------------------------
        public void ChangeLane (Sequence action, Lane lane, float duration)
        {
            if (lane == null) return;

            // No Lane changing when car doesn't move
            if (Car.Velocity < 10) return;

            // Debug
            if (this is Police) 
            {
//                duration *= 5;
//                Debugger.Break();
            }

            // Add to new Lane
            action.Add (new Generic (() => lane.Add (Car)));

            #region Debug
            if (Settings.NoChangeLaneAnimation)
            {
                action.Add (new Generic (DockToLane));
                return;
            }
            #endregion

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

            // Fix accuracy error in Car's Position
            action.Add (new Generic (DockToLane));
        }

        //------------------------------------------------------------------
        private void DockToLane()
        {
            Car.LocalPosition = new Vector2 (0, Car.Position.Y);
        }

        #endregion

        #region Debug

        //-----------------------------------------------------------------
        private void Debug()
        {
//            DrawSafeZone ();
//            DrawActions ();
//            DrawCheckLane (Car.Lane);

//            new Text (SafeZone.Scale.ToString(), Position, Color.Orange, true);
        }

        //------------------------------------------------------------------
        public void DrawSafeZone()
        {
            var pos = Car.Position;

            float body = 50;
            float red = SafeZone.HighDanger - body;
            float orange = SafeZone.LowDanger - body;

            new Line (pos, pos - new Vector2 (0, orange), Color.Orange);
            new Line (pos, pos + new Vector2 (0, orange), Color.Orange);
            new Line (pos, pos - new Vector2 (0, red), Color.Maroon);
            new Line (pos, pos + new Vector2 (0, red), Color.Maroon);

//            new Text (SafeZone.Scale.ToString(), Position, Color.SteelBlue, 20);

            return;
            float lenght = orange - body;
            new Line (pos, pos - new Vector2 (0, lenght), Color.SlateBlue);
            new Line (pos, pos + new Vector2 (0, lenght), Color.SlateBlue);
        }

        //-----------------------------------------------------------------
        public void DrawCheckLane (Lane lane)
        {
            if (lane == null) return;

            var shift = new Vector2 ((Car.Position - lane.Position).X + 20, 0);

            if (CheckLane (lane))
                new Text ("Free", Car.Position - shift, Color.ForestGreen);
            else
                new Text ("Danger", Car.Position - shift, Color.Maroon);
        }

        //------------------------------------------------------------------
        protected void DrawActions()
        {
            var actionsNames = Loop.Actions.Aggregate ("\n", (current, action) => current + (action + "\n"));
            new Text (actionsNames, Car.Position, Color.SteelBlue, true);
        }

        //------------------------------------------------------------------
        protected void WriteActions()
        {
            var actionsNames = Loop.Actions.Aggregate ("", (current, action) => current + (action + "\n"));
            Console.WriteLine (actionsNames);
        }

        //------------------------------------------------------------------
//        private void DrawCheckLane (Lane lane)
//        {
//            if (lane == null) return;
//
//            var pos = lane.Position;
//            pos.Y = Car.Position.Y;
//
//            new Line (pos, pos - new Vector2 (0, SafeZone.Calculate () / 2), Color.IndianRed);
//            new Line (pos, pos + new Vector2 (0, SafeZone.Calculate () * 1.5f), Color.Orange);
//        }

        #endregion
    }
}