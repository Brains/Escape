using System;
using System.Linq;
using Traffic.Actions;
using Traffic.Cars;

namespace Traffic.Drivers
{
    internal class Common : Driver
    {
        //------------------------------------------------------------------
        public Common (Car car) : base(car)
        {
            AddParallel (new Shrink (this));
        }
    }
}