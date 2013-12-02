using Traffic.Actions.Base;
using Traffic.Drivers;

namespace Traffic.Actions
{
    public class ChangeLane : Sequence
    {
        private readonly Driver driver;
        private readonly Lane lane;

        //------------------------------------------------------------------
        public ChangeLane (Driver driver, Lane lane)
        {
            this.driver = driver;
            this.lane = lane;
            Add (new Generic (Perform));
        }

        //------------------------------------------------------------------
        private void Perform()
        {
            if (driver.TryChangeLane (this, lane, driver.GetChangeLanesDuration()))
                driver.Car.EnableBlinker (lane);
        }
    }
}