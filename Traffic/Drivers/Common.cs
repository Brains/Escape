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
            float distance = GetMinimumDistance (Car.Lane.Cars.Where (IsAhead));

            if (distance < DangerousZone)
            {
                AvoidCollisions ();
            }
        }
    }
}