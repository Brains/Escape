using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Police : Driver
    {
        public override float DangerousZone
        {
            // Hardcoded numbers are constants
            get { return Car.Lenght * Car.Velocity / 200; }
        }

        //------------------------------------------------------------------
        public Police (Cars.Police police) : base (police)
        {
            Velocity = 500;

            AddParallel (new Shrink (this));
            AddParallel (new Overtake (this, Car.Lane.Road.Player));
        }
    }
}