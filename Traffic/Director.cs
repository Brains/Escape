using Traffic.Actions;
using Traffic.Cars;

namespace Traffic
{
    public class Director
    {
        private readonly Manager manager;

        //-----------------------------------------------------------------
        public Director (Manager manager)
        {
            this.manager = manager;
        }

        #region Events

        //-----------------------------------------------------------------
        public void Setup ()
        {
//            Tools.Timers.Loop.Create (1, 0, ChangeMaximumCarsOnLaneEvent);
//            Tools.Timers.Loop.Create (5, 0, ChangeLaneForCarEvent);
            Tools.Timers.Loop.Create (0, 1, CreatePolice);
        }

        //-----------------------------------------------------------------
        private void ChangeLaneForCarEvent ()
        {
            // To Left
            var car = GetRandomCar ();
            car.Driver.AddInSequnce (new TryChangeLane (car.Driver, car.Lane.Left));

            // To Right
            car = GetRandomCar ();
            car.Driver.AddInSequnce (new TryChangeLane (car.Driver, car.Lane.Right));
        }

        //-----------------------------------------------------------------
        private void ChangeMaximumCarsOnLaneEvent ()
        {
            var lane = GetRandomLane ();
            lane.CarsQuantity = Lane.Random.Next (Lane.MinimumCars, Lane.MaximumCars);
        }

        #endregion


        #region Helpers

        //-----------------------------------------------------------------
        private Lane GetRandomLane ()
        {
            var laneID = Lane.Random.Next (Road.LanesQuantity);
            Lane lane = (Lane) manager.Road.Components[laneID];

            return lane;
        }

        //------------------------------------------------------------------
        private Car GetRandomCar ()
        {
            var lane = GetRandomLane ();

            // Find correct Car on road
            Car car;

            do
                car = GetRandomCarOnLane (lane);
            while (!IsValid (car));

            return car;
        }

        //------------------------------------------------------------------
        private static Car GetRandomCarOnLane (Lane lane)
        {
            var carID = Lane.Random.Next (lane.CarsQuantity);

            // If Lane hasn't append cars yet
            if (carID >= lane.Cars.Count) 
                carID = lane.Cars.Count - 1;

            return lane.Cars[carID];
        }

        //-----------------------------------------------------------------
        private bool IsValid (Car car)
        {
            return car.GetType () == typeof (Car);
        }

        //-----------------------------------------------------------------
        private void CreatePolice ()
        {
            var lane = GetRandomLane ();
            lane.CreatePolice (manager.Game);
        }


        #endregion
    }
}