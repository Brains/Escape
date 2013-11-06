using System;
using System.Linq;
using Android.OS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Traffic.Cars;
using Traffic.Helpers;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Player : Common
    {
        //------------------------------------------------------------------
        public Player (Car car) : base (car) {}

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

//            AdjustSpeed ();

            UpdateInput ();

            Debug ();
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
            if (KeyboardInput.IsKeyPressed (Keys.Right)) Car.Driver.ChangeLane (Car.Lane.Right);
            if (KeyboardInput.IsKeyPressed (Keys.Left)) Car.Driver.ChangeLane (Car.Lane.Left);
            if (KeyboardInput.IsKeyDown (Keys.Down)) Car.Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) /*Car.Accelerate ();*/ ForceAccelerate ();
        }

        //------------------------------------------------------------------
        protected void ForceAccelerate ()
        {
            Car.Velocity += Car.Acceleration;
        }

        //-----------------------------------------------------------------
        private void Debug ()
        {
            string actionsNames = "";

            foreach (var action in Actions)
                actionsNames += action + "\n";


            new Text (actionsNames, Car.GlobalPosition, Color.DarkRed, true);

//            Console.WriteLine (actionsNames + "\n\n\n");
        }
    }
}