using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Traffic.Helpers;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Player : Driver
    {

        //------------------------------------------------------------------
        public Player (Car car)
        {
            Car = car;
            Velocity = 400;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            AdjustSpeed ();

            UpdateInput ();

        }

        //-----------------------------------------------------------------
        private void AdjustSpeed ( )
        {
            float distance = GetMinimumDistance (Car.Lane.Cars.Where (IsAhead));

            if (distance > 600) return;
            else if (distance > 300) Car.Accelerate ();
            else if (distance < 300) Car.Brake();


//            float factor = distance / (DangerousZone);
//            Car.Velocity *= factor;

//            new Text (factor.ToString ("F3"), Car.GlobalPosition, Color.Red);
        }

        //------------------------------------------------------------------
        public void UpdateInput ()
        {
            if (KeyboardInput.IsKeyPressed (Keys.Right)) Car.ChangeLane (Car.Lane.Right);
            if (KeyboardInput.IsKeyPressed (Keys.Left)) Car.ChangeLane (Car.Lane.Left);
            if (KeyboardInput.IsKeyDown (Keys.Down)) Car.Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) ForceAccelerate ();
        }

        //------------------------------------------------------------------
        protected void ForceAccelerate ()
        {
            Car.Velocity += Car.Acceleration;
        }
    }
}