using System;
using System.Linq;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    public class SpeedControl : Base.Sequence
    {
        Driver driver;
        
        //------------------------------------------------------------------
        public SpeedControl(Driver driver)
        {
            this.driver = driver;
            Name = "Speed Control";

            Add (new Generic (Start));
        }

        //------------------------------------------------------------------
        public override Base.Action Copy()
        {
            return new SpeedControl (driver);
        }

        //------------------------------------------------------------------
        public void Start ()
        {
            Car closest = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsAhead));

            if (driver.Distance (closest) > driver.GetSafeZone ())
                driver.Accelerate (this);
        }
    }
}