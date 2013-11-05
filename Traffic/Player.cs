using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Traffic.Helpers;

namespace Traffic
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
            Acceleration = 5;
        }
    }
}