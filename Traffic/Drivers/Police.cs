using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Police : Driver
    {
        //------------------------------------------------------------------
        public Police (Cars.Police police) : base (police)
        {
            AddParallel (new Shrink (this));
            AddParallel (new Overtake (this, Car.Lane.Road.Player));
        }
    }
}