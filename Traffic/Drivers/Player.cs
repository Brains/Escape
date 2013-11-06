using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Traffic.Cars;
using Traffic.Helpers;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Player : Driver
    {
        private float velocity = 400;

        //------------------------------------------------------------------
        public override float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //------------------------------------------------------------------
        public Player (Car car)
        {
            Car = car;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            AdjustSpeed ();

            UpdateInput ();
        }

        //-----------------------------------------------------------------
        private void AdjustSpeed ()
        {
            float distance = GetMinimumDistance (Car.Lane.Cars.Where (IsAhead));

            if (distance < 600)
                Car.Velocity += distance / 300 - 1.5f;

//            if (distance > 300) Accelerate ();
//            else if (distance < 100) Brake ();


//            float factor = distance / (DangerousZone);
//            Car.Velocity *= factor;

//            new Text (factor.ToString ("F3"), Car.GlobalPosition, Color.Red);
        }

        //------------------------------------------------------------------
        private void Accelerate ()
        {
            if (Car.Velocity < 300)
                Car.Velocity += 1.0f;
        }

        //------------------------------------------------------------------
        private void Brake ()
        {
            if (Car.Velocity > 150)
                Car.Brake ();

//            if (Car.Velocity > 150)
//                Car.Velocity -= 5.0f;
        }

        //------------------------------------------------------------------
        public void UpdateInput ()
        {
            if (KeyboardInput.IsKeyPressed (Keys.Right)) Car.ChangeLane (Car.Lane.Right);
            if (KeyboardInput.IsKeyPressed (Keys.Left)) Car.ChangeLane (Car.Lane.Left);
            if (KeyboardInput.IsKeyDown (Keys.Down)) Car.Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) /*Car.Accelerate ();*/ ForceAccelerate ();
        }

        //------------------------------------------------------------------
        protected void ForceAccelerate ()
        {
            Car.Velocity += Car.Acceleration;
        }
    }
}