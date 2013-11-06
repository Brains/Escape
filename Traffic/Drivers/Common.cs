using System.Linq;
using Traffic.Actions;
using Traffic.Cars;

namespace Traffic.Drivers
{
    internal class Common : Driver
    {
        private Sequence dodge;

        //------------------------------------------------------------------
        public Common (Car car) : base(car)
        {
//            Add (new Loop (Car.Accelerate));
//            Add (new Loop (AvoidCollisions));

            dodge = new Sequence {Name = "Dodge"};
            Add (dodge);
            StartDodge ();
        }

        //------------------------------------------------------------------
        private void StartDodge ()
        {
            DetectDanger ();

            // Provide looping for the action
            dodge.Add (new Generic (StartDodge) { Name = "Start" });
        }

        //------------------------------------------------------------------
        private void DetectDanger ()
        {
            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsAhead));
            if (closestCar == null)
            {
                Accelerate ();
                return;
            }

            // Set safely distance
            if (Distance (closestCar) < 200)
                Brake ();



            if (Distance (closestCar) > 300)
                Accelerate ();


            if (Velocity <= closestCar.Velocity) return;

            CalculateDangerousZone ();
            if (Distance (closestCar) > DangerousZone) return;


        }

        //------------------------------------------------------------------
        private void Brake ()
        {
            dodge.Add (new Repeated (Car.Brake, 10) {Name = "Brake"});
        }

        //------------------------------------------------------------------
        private void Accelerate ()
        {
            dodge.Add (new Repeated (Car.Accelerate, 5) {Name = "Accelerate"});
        }
    }
}