using Microsoft.Xna.Framework;
using Traffic.Drivers;

namespace Traffic.Cars
{
    internal class Police : Car
    {
                //------------------------------------------------------------------
        public Police (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Police (this);
            InitialColor = Color.White;
            TextureName = "Police";
            Lives = 99;
            Acceleration = 1.0f;
        } 
    }
}