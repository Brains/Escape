using Traffic.Actions.Base;
using Traffic.Drivers;

namespace Traffic.Actions
{
    public class TryChangeLane : SequenceInitial
    {
        private readonly Driver driver;
        private readonly Lane lane;

        //------------------------------------------------------------------
        public TryChangeLane (Driver driver, Lane lane)
        {
            this.driver = driver;
            this.lane = lane;
            Initial = new Generic (ChangeLane);
        }

        //------------------------------------------------------------------
        private void ChangeLane()
        {
            if (driver.TryChangeLane (this, lane, driver.GetChangeLanesDuration()))
                driver.Car.EnableBlinker (lane);
        }
    }
}