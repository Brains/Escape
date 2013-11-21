using System;
using Microsoft.Xna.Framework;

namespace Traffic.Cars
{
    internal class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Player (this);
            Lives = 99;

            TextureName = "Player";
            InitialColor = Color.White;

            Velocity = 100;
            Acceleration = 0.4f;
            Deceleration = 1.0f;
        }
    }
}