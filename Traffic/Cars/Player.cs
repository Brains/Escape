using System;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Cars.Weights;

namespace Traffic.Cars
{
    public class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int id, int position, Weight weight, string textureName)
            : base (lane, id, position, weight, textureName)
        {
            Lives = 80;
            Velocity = 300;
//            Acceleration = 1; 
            Acceleration = 0.3f;
//            Deceleration = 2; 
            Deceleration = 1.0f;

            Driver = new Drivers.Player (this);
        }

        public override void Update (float elapsed)
        {
            base.Update (elapsed);

//            InteractOnFluid();
        }
    }
}