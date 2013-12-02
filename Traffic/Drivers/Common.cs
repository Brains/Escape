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
            AddInLoop (new Shrink (this));
            AddInLoop (new SpeedControl(this));
        }
    }
}