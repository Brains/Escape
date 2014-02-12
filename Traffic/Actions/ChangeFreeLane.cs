using Microsoft.Xna.Framework;
using Traffic.Actions.Base;
using Traffic.Drivers;

namespace Traffic.Actions
{
    public class ChangeFreeLane : Sequence
    {
        private readonly Driver driver;

        //------------------------------------------------------------------
        public ChangeFreeLane (Driver driver)
        {
            this.driver = driver;
            Add (new Generic (Perform));

//            driver.Car.Color = Color.DarkGray;
        }

        //------------------------------------------------------------------
        private void Perform()
        {
            if (TryChangeLane (driver.Car.Lane.Left))
                return;
            if (TryChangeLane (driver.Car.Lane.Right))
                return;
        }

        //------------------------------------------------------------------
        private bool TryChangeLane (Lane lane)
        {
            if (driver.TryChangeLane (this, lane, driver.GetChangeLanesDuration())) {

                driver.Car.EnableBlinker (lane);
                return true;
            }

            return false;
        }
    }
}