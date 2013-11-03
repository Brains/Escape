using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools;
using Tools.Extensions;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Common : Driver
    {
        private float cruiseZone;

        //------------------------------------------------------------------
        public Common (Car car)
        {
            Car = car;
        }

        //------------------------------------------------------------------
        public override void Create ()
        {
            base.Create ();

            cruiseZone = Car.Lenght * 4.0f;
        }

        //------------------------------------------------------------------
        public override void Update ()
        {
            Car.Accelerate ();

            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsAhead));
            if (closestCar == null) return;

            if (Velocity > closestCar.Velocity && Distance (closestCar) < DangerousZone)
            {
                AvoidCollisions ();
            }
        }

        //-----------------------------------------------------------------
        private void ChangeLane ()
        {
            ChangeLane (Lane.Random.Next (2) == 0 ? Car.Lane.Left : Car.Lane.Right);
        }
    }
}