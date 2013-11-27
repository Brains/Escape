using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Traffic.Actions;
using Traffic.Actions.Base;
using Traffic.Cars;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Player : Driver
    {
        //------------------------------------------------------------------
        public Player (Car car) : base (car)
        {
            Velocity = 300;

            AddInLoop (new Input (this));
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

//            AdjustSpeed ();

            Debug ();
        }

        //-----------------------------------------------------------------
        private void AdjustSpeed ()
        {
            float distance = GetMinimumDistance (Car.Lane.Cars.Where (IsAhead));

            // A point of the "factor" is to accelerate when (distance > Lenght * 3)
            int factor = Math.Sign (distance / Car.Lenght - 3);

//            float factor = (distance / Car.Lenght - 3) / 9;
//            if (factor > 1) factor = 1.0f;
//            if (factor < -1) factor = -1.0f;
//            Car.Velocity += Car.Acceleration * factor;
//            new Text (factor.ToString (), Vector2.One * 100, Color.DarkViolet);

            if (factor > 0) 
                Accelerate ();
            else
                Brake ();
        }

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            if (Car.Velocity < Velocity) 
            {
                Car.Accelerate ();
                Car.EnableBoost ();
            }
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Car.Velocity > 100)
                Car.Brake ();
        }

        //-----------------------------------------------------------------
        private void Debug ()
        {
//            new Text (Car.Velocity, Car.GlobalPosition, Color.DarkRed, true);

            // Draw Closest car
//            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsAhead));
//            if (closestCar != null)
//                closestCar.Color = Color.Red;
        }

    }
}