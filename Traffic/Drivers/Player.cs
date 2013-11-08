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
            Velocity = 400;

            AddParallel (new Input (this));
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

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
        public void Accelerate ()
        {
            if (Car.Velocity < Velocity)
                Car.Accelerate ();
        }

        //-----------------------------------------------------------------
        private void Debug ()
        {
//            new Text (Car.Velocity, Car.GlobalPosition, Color.DarkRed, true);


            string actionsNames = Actions.Aggregate ("", (current, action) => current + (action + ""));
//            new Text (actionsNames, Car.GlobalPosition, Color.DarkRed, true);
//            Console.WriteLine (actionsNames);


//            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsAhead));
//            if (closestCar != null)
//                closestCar.Color = Color.Red;
        }

    }
}