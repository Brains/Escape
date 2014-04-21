using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Tools.Timers;

namespace Traffic.Cars
{
    public class Blinker : Lights
    {
        private Loop timer;

        //------------------------------------------------------------------
        public Blinker(Car car, string textureName) : base(car, textureName) {}

        //------------------------------------------------------------------
        public override void Enable()
        {
            base.Enable();

            CreateBlinker();
        }

        //------------------------------------------------------------------
        private void CreateBlinker ()
        {
            timer = new Loop();
            timer.Interval = 0.5f;
            timer.IterationsLimit = 10;
            timer.Trigger = Turn;
            timer.Finish += Disable;
        }

        //------------------------------------------------------------------
        public override void Turn()
        {
            Drawable.Color = Drawable.Color == Color.White ? Color.Transparent : Color.White;
        }

        //------------------------------------------------------------------
        public override void Disable()
        {
            base.Disable();

            timer = null;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            if (timer != null) 
                timer.Update (elapsed);
        }
    }
}