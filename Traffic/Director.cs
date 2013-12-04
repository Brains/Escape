using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions;
using Traffic.Cars;
using Traffic.Drivers;
using Police = Traffic.Cars.Police;

namespace Traffic
{
    public class Director
    {
        private readonly Manager manager;
        private List <Police> polices;

        //-----------------------------------------------------------------
        public Director (Manager manager)
        {
            this.manager = manager;
            polices = new List <Police>();
        }

        #region Events

        //-----------------------------------------------------------------
        public void Setup()
        {
            Tools.Timers.Loop.Create (10, 0, ChangeMaximumCarsOnLaneEvent);
            Tools.Timers.Loop.Create (0.3f, 0, ChangeLaneForCarEvent);
            Tools.Timers.Loop.Create (0, 0, CreatePolice);
            Tools.Timers.Loop.Create (5, 0, CreateBlock);
        }

        //-----------------------------------------------------------------
        private void ChangeLaneForCarEvent()
        {
            // To Left
            var car = GetRandomCar();
            car.Driver.AddInSequnce (new ChangeLane (car.Driver, car.Lane.Left));

            // To Right
            car = GetRandomCar();
            car.Driver.AddInSequnce (new ChangeLane (car.Driver, car.Lane.Right));
        }

        //-----------------------------------------------------------------
        private void ChangeMaximumCarsOnLaneEvent()
        {
            var lane = GetRandomLane();
            lane.CarsQuantity = Lane.Random.Next (Lane.MinimumCars, Lane.MaximumCars);
        }

        //------------------------------------------------------------------
        private void CreateBlock()
        {
            Driver player = manager.Road.Player.Driver;
            IEnumerable <Car> aheadCars = player.Car.Lane.Cars.Where (player.IsCarAhead);
            Car closest = player.FindClosestCar (aheadCars);
            if (closest == null) return;
            if (player.Distance (closest) > 300) return;

            var block = new Block (closest.Driver, player.Car);
            if (!closest.Driver.IsInLoop (block))
                closest.Driver.AddInLoop (block);
        }

        #endregion

        #region Helpers

        //-----------------------------------------------------------------
        private Lane GetRandomLane()
        {
            var laneID = Lane.Random.Next (Road.LanesQuantity);
            Lane lane = (Lane) manager.Road.Components[laneID];

            return lane;
        }

        //------------------------------------------------------------------
        private Car GetRandomCar()
        {
            var lane = GetRandomLane();

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
            return car.GetType() == typeof (Car);
        }

        //-----------------------------------------------------------------
        private void CreatePolice()
        {
            if (polices.Count > 0) return;

            var car = GetRandomLane().CreatePolice (manager.Game);
            polices.Add (car);
        }

        //------------------------------------------------------------------
        public void Update (float elapsed)
        {
            polices.RemoveAll (car => car.Deleted);
        }

        #endregion
    }
}