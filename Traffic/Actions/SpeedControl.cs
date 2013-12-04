using System;
using System.Linq;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    public class SpeedControl : SequenceInitial
    {
        Driver driver;
        
        //------------------------------------------------------------------
        public SpeedControl (Driver driver)
        {
            this.driver = driver;
            Initial = new Generic (Start);
        }

        //------------------------------------------------------------------
        public void Start ()
        {
            Car closest = driver.FindClosestCar (driver.Car.Lane.Cars.Where (driver.IsCarAhead));

            if (driver.Distance (closest) > driver.SafeZone.LowDanger)
                driver.Accelerate (this);
        }
    }
}