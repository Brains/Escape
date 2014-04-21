using System;
using Microsoft.Xna.Framework;
using Traffic.Cars.Weights;

namespace Traffic.Cars
{
    public class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int id, int position) : base (lane, id, position)
        {
            Lives = 80;
            Velocity = 300;
//            Acceleration = 1; 
            Acceleration = 0.3f;
//            Deceleration = 2; 
            Deceleration = 1.0f;

            SetDriver (new Drivers.Player (this));
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            CreateDrawable (game, "Player");

            base.Setup (game);
        }
    }
}