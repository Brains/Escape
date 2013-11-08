using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Police : Driver
    {
        //------------------------------------------------------------------
        public Police (Cars.Police police) : base (police)
        {
//            Add (new Shrink (this));
            Add (new Overtake (this, Car.Lane.Road.Player));
        }
    }
}