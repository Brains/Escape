using System;
using Microsoft.Xna.Framework;
using Traffic.Cars.Weights;

namespace Traffic.Cars
{
    public class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int id, int position)
            : base (lane, id, position)
        {
            Lives = 99;
            Velocity = 300;
            Acceleration = 1;//0.3f;
            Deceleration = 2;//1.0f;

            Driver = new Drivers.Player (this);
        }
    }
}