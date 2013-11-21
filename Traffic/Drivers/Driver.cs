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

namespace Traffic.Drivers
{
    internal abstract class Driver
    {
        //------------------------------------------------------------------
        protected Loop Loop = new Loop ();

        //------------------------------------------------------------------
        public Car Car { get; set; }
        public float Velocity { get; set; }

        public virtual float DangerousZone
        {
            // Hardcoded numbers are constants for normalization
            get { return Car.Lenght + (Car.Lenght * Car.Velocity / 200); }
        }

        //------------------------------------------------------------------
        protected Driver (Car car)
        {
            Car = car;
        }

        #region Actions

        //-----------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            Loop.Update (elapsed);

            Debug ();
        }

        //------------------------------------------------------------------
        public void AddInLoop (Sequence action)
        {
            Loop.Add (action);
        }

        #endregion

        #region Sensor Analysis

        //------------------------------------------------------------------
        public float Distance (Car car)
        {
            // Don't react with own Car
            if (car == null || car == Car) return float.MaxValue;

            var distance = Car.GlobalPosition - car.GlobalPosition;

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
            return car.GlobalPosition.Y < Car.GlobalPosition.Y;
        }

        //------------------------------------------------------------------
        public bool CheckLane (Lane lane)
        {
            if (lane == null) return false;

            if (GetMinimumDistance (lane.Cars) < DangerousZone / 1.5)
                return false;

            return true;
        }

        //-----------------------------------------------------------------
        public float GetChangeLanesDuration ( )
        {
            return 400.0f / Car.Velocity;
        }

        #endregion

        #region Car Controll

        //------------------------------------------------------------------
        public void Brake (Composite action, int times)
        {
            action.Add (new Repeated (Car.Brake, times) {Name = "Brake"});
        }

        //------------------------------------------------------------------
        public void Accelerate (Composite action, int times)
        {
            if (Car.Velocity < Car.Lane.Velocity)
                action.Add (new Repeated (Car.Accelerate, times) {Name = "Accelerate"});
        }

        //------------------------------------------------------------------
        public void EnableBlinker (Lane lane, Composite action, float delay)
        {
            Car.EnableBlinker (lane);

            action.Add (new Sleep (delay));
        }

        //------------------------------------------------------------------
        private void DisableBlinker (Composite action)
        {
            action.Add (new Generic (() => Car.DisableBlinker ()));
        }

        //------------------------------------------------------------------
        public bool TryChangeLane (Lane lane, Sequence action, float duration)
        {
            if (CheckLane (lane))
            {
                EnableBlinker (lane, action, duration / 2.0f);
                ChangeLane (lane, action, duration / 2.0f);
                DisableBlinker (action); 
                return true;
            }

            return false;
        }

        //------------------------------------------------------------------
        public void ChangeLane (Lane lane, Sequence action, float duration)
        {
            if (lane == null) return;

            // No Lane changing when car doesn't move
            if (Car.Velocity < 10) return;

            // Rotate
            Action <float> rotate = share => Car.Angle += share;
            float finalAngle = MathHelper.ToRadians ((lane.GlobalPosition.X < Car.GlobalPosition.X) ? -10 : 10);
            action.Add (new Controller (rotate, finalAngle, duration * 0.1f));

            // Moving
            Action <Vector2> move = shift => Car.Position += shift;
            var diapason = new Vector2 (lane.GlobalPosition.X - Car.GlobalPosition.X, 0);
            action.Add (new Controller (move, diapason, duration * 0.2f));

            // Inverse rotating
            var inverseRotating = new Controller (rotate, -finalAngle, duration * 0.1f);
            action.Add (inverseRotating);

            // Add to new Lane
            action.Add (new Generic (() => lane.Add (Car)));
        }

        #endregion

        //-----------------------------------------------------------------
        private void Debug ( )
        {
//            Console.WriteLine (Loop);

//            if (!(this is Police)) return;

            // Check Left Lane
//            var pos = Car.Lane.Left.GlobalPosition;
//            pos.Y = Car.GlobalPosition.Y;
//            new Line (pos, pos - new Vector2 (0, DangerousZone / 1.5f), Color.IndianRed);
//            new Line (pos, pos + new Vector2 (0, DangerousZone / 1.5f), Color.IndianRed);

            // Check Right Lane
//            pos = Car.Lane.Right.GlobalPosition;
//            pos.Y = Car.GlobalPosition.Y;
//            new Line (pos, pos - new Vector2 (0, DangerousZone / 1.5f), Color.IndianRed);
//            new Line (pos, pos + new Vector2 (0, DangerousZone / 1.5f), Color.IndianRed);
        }
    }
}