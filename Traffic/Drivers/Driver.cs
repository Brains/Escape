using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Extensions;
using Tools.Markers;
using Traffic.Actions;

namespace Traffic.Drivers
{
    abstract class Driver
    {
        //------------------------------------------------------------------
        protected Car Car;
        protected List<Actions.Action> Actions = new List<Actions.Action> ();
        protected float DangerousZone;
        public float Velocity { get; set; }

        //------------------------------------------------------------------
        public virtual void Update ()
        {
//            float elapsed = /*1 / 60*/; //(float) gameTime.ElapsedGameTime.TotalSeconds;

//            Actions.ForEach (action => action.Update (elapsed));

            Actions.RemoveAll (actions => actions.Finished);
        }

        //------------------------------------------------------------------
        public virtual void Create ()
        {
            DangerousZone = Car.Lenght * 1.5f;
        }

        //------------------------------------------------------------------
        protected void AvoidCollisions ()
        {
            if (ChangeLane (Car.Lane.Left)) return;
            if (ChangeLane (Car.Lane.Right)) return;
            Car.Brake ();
        }

        //------------------------------------------------------------------
        protected bool ChangeLane (Lane lane)
        {
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

            var distance = Car.Position - car.Position;

            return Math.Abs (distance.Y);
        }

        //------------------------------------------------------------------
        protected float GetMinimumDistance (IEnumerable<Car> cars)
        {
            if (!cars.Any ()) return float.MaxValue;

            return cars.Min <Car, float> (Distance);
        }

        //------------------------------------------------------------------
        protected bool IsAhead (Car car)
        {
            return car.Position.Y < Car.Position.Y;
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
        public Car FindClosestCar (IEnumerable<Car> cars)
        {
            return cars.MinBy <Car, float> (Distance);
        }

        #endregion
    }
}