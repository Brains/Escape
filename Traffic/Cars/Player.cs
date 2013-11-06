using Microsoft.Xna.Framework;

namespace Traffic.Cars
{
    internal class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Player (this);
            InitialColor = Color.White;
            TextureName = "Player";
            Lives = 99;
            Acceleration = 0.5f;
        }
    }
}