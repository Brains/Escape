using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Extensions;
using Tools.Markers;
using Traffic.Actions;
using Traffic.Cars;

namespace Traffic.Drivers
{
    internal abstract class Driver
    {
        //------------------------------------------------------------------
        protected Car Car;
        protected List <Actions.Action> Actions = new List <Actions.Action> ();
        protected List <Actions.Action> ActionsToAdd = new List <Actions.Action> ();
        protected float DangerousZone;
        protected bool Locked;

        public virtual float Velocity { get; set; }

        //------------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            Locked = false;

            foreach (var action in Actions)
            {
                action.Update (elapsed);

                if (action.Lock) Locked = true;
            }

            AddDeferredActions ();
            Actions.RemoveAll (actions => actions.Finished);
        }

        //------------------------------------------------------------------
        private void AddDeferredActions ()
        {
            // Insert in first position to detect Lock property as early as possible
            ActionsToAdd.ForEach (action => Actions.Insert (0, action));

            ActionsToAdd.Clear ();
        }

        //------------------------------------------------------------------
        protected void AvoidCollisions ()
        {
            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsAhead));
            if (closestCar == null) return;

            if (Velocity <= closestCar.Velocity) return;

            CalculateDangerousZone ();
            if (Distance (closestCar) > DangerousZone) return;

            // Avoid Danger situation
            if (ChangeLane (Car.Lane.Left)) return;
            if (ChangeLane (Car.Lane.Right)) return;
            Car.Brake ();
        }

        //------------------------------------------------------------------
        protected bool ChangeLane (Lane lane)
        {
            Car.EnableBlinker (lane);

            var sequence = new Actions.Sequence ();
            sequence.Add (new Sleep (1.0f));
//            sequence.Add (new Generic (() => Car.Blinker.Disable ()));
            Add (sequence);

//            Add (new Repeated (() => Add (sequence), 10));

            if (CheckLane (lane))
            {
                Car.ChangeLane (lane);
                return true;
            }

            return false;
        }

        #region Sensor Analysis

        //------------------------------------------------------------------
        protected float Distance (Car car)
        {
            // Don't react with own Car
            if (car == Car) return float.MaxValue;

            var distance = Car.GlobalPosition - car.GlobalPosition;

            return Math.Abs (distance.Y);
        }

        //------------------------------------------------------------------
        protected float GetMinimumDistance (IEnumerable <Car> cars)
        {
            if (!cars.Any ()) return float.MaxValue;

            return cars.Min <Car, float> (Distance);
        }

        //------------------------------------------------------------------
        protected bool IsAhead (Car car)
        {
            return car.GlobalPosition.Y < Car.GlobalPosition.Y;
        }

        //------------------------------------------------------------------
        public bool CheckLane (Lane lane)
        {
            if (lane == null) return false;

            if (GetMinimumDistance (lane.Cars) < DangerousZone)
                return false;

            return true;
        }

        //------------------------------------------------------------------
        public Car FindClosestCar (IEnumerable <Car> cars)
        {
            return cars.MinBy (Distance);
        }

        //------------------------------------------------------------------
        protected void CalculateDangerousZone ()
        {
            DangerousZone = Car.Lenght * Car.Velocity / 60.0f;
//            new Line (Car.GlobalPosition, Car.GlobalPosition + new Vector2 (0, -DangerousZone));
        }

        #endregion

        #region Actions

        //-----------------------------------------------------------------
        public void Add (Actions.Action action)
        {
            if (!Locked)
                ActionsToAdd.Add (action);
        }

        //-----------------------------------------------------------------
//        public void Remove (Actions.Action action)
//        {
//            Actions.Remove (action);
//        }

        #endregion
    }
}