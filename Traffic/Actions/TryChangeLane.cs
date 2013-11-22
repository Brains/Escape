using Traffic.Actions.Base;
using Traffic.Drivers;

namespace Traffic.Actions
{
    public class TryChangeLane : Sequence
    {
        private readonly Driver driver;
        private readonly Lane lane;

        //------------------------------------------------------------------
        public TryChangeLane (Driver driver, Lane lane)
        {
            this.driver = driver;
            this.lane = lane;
            Name = "TryChangeLane";

            Add (new Generic (() => this.driver.TryChangeLane (this.lane, this, this.driver.GetChangeLanesDuration ())));
        }

        //------------------------------------------------------------------
        public override Action Copy ()
        {
            return new TryChangeLane (driver, lane);
        }
    }
}