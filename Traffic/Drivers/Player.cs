using System;
using System.Linq;
using Traffic.Actions;
using Traffic.Cars;
using Tools.Markers;

namespace Traffic.Drivers
{
    internal class Player : Common
    {
        //------------------------------------------------------------------
        public Player (Car car) : base (car)
        {
            Add (new Input (this));
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





        //-----------------------------------------------------------------
        private void Debug ()
        {
            string actionsNames = "";

            foreach (var action in Actions)
                actionsNames += action + "";

//            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsAhead));
//            if (closestCar != null)
//                closestCar.Color = Color.Red;


//            new Text (actionsNames, Car.GlobalPosition, Color.DarkRed, true);

//            Console.WriteLine (actionsNames);
        }
    }
}