using Microsoft.Xna.Framework;
using Traffic.Drivers;

namespace Traffic.Cars
{
    public class Police : Car
    {
                //------------------------------------------------------------------
        public Police (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Police (this);
            InitialColor = Color.White;
            TextureName = "Police";
            Lives = 9;
            Acceleration = 1.5f;
            Deceleration = 2.0f;
        } 
    }
}