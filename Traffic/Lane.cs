using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Cars;

namespace Traffic
{
    internal class Lane : Object
    {
        private class Attributes
        {
            public int ID;
            public int MaximumCars = 3;
            public int Height;
            public List <Car> CarsToAdd;
        }

        private Attributes Properties = new Attributes ();
        private static int carsCounter;

        //------------------------------------------------------------------
        public List <Car> Cars { get; private set; }
        public int Velocity { get; set; }
        public Lane Left { get; set; }
        public Lane Right { get; set; }
        public static Random Random { get; set; }
        public Road Road { get; private set; }

        #region Creation

        //------------------------------------------------------------------
        static Lane ()
        {
            Random = new Random ();
        }

        //------------------------------------------------------------------
        public Lane (Road road, int id) : base (road)
        {
            Properties.ID = id;
            Road = road;
            Anchored = true;
            CalculatePosition (Properties.ID);
            CalculateVelocity (Properties.ID);

            Cars = new List <Car> ();
            Properties.CarsToAdd = new List <Car> ();
        }

        //------------------------------------------------------------------
        private void CalculateVelocity (int id)
        {
            int maximumVelocity = 240;
            int step = 20;
            Velocity = maximumVelocity - id * step;
        }

        //------------------------------------------------------------------
        private void CalculatePosition (int id)
        {
            const int laneWidth = 40;
            int position = id * laneWidth + laneWidth / 2;

            Position = new Vector2 (position, 0);
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            Properties.Height = Road.Game.GraphicsDevice.Viewport.Height;
            
            base.Setup ();
        }

        //------------------------------------------------------------------
        private void CreateCar ()
        {
            var car = new Car (this, GetInsertionPosition ()) {ID = carsCounter};
            car.Setup ();

            Cars.Add (car);
            OwnCar (car);

            carsCounter++;
        }

        //------------------------------------------------------------------
        public Player CreatePlayer (Game game)
        {
            var player = new Player (this, 400) {ID = carsCounter};
            player.Setup ();
            
            Cars.Add (player);
            OwnCar (player);

            carsCounter++;

            return player;
        }

        //------------------------------------------------------------------
        public void CreatePolice (Game game)
        {
            var police = new Police (this, 600) { ID = carsCounter };
            police.Setup ();

            Cars.Add (police);
            OwnCar (police);

            carsCounter++;
        }

        //------------------------------------------------------------------
        // Return point outside the screen
        private int GetInsertionPosition ()
        {
            float playerVelocity = (Road.Player != null) ? Road.Player.Velocity : 0;
            int shift = (Velocity > playerVelocity) ? Properties.Height : -Properties.Height;

            return (int) (Random.NextDouble () * Properties.Height + shift);
        }

        #endregion

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            AddQueuedCars ();

            CleanUp ();
            AppendCars ();

            Components.Clear ();
            Components.AddRange (Cars);

            Debug ();
        }

        #region Cars Management

        //------------------------------------------------------------------
        private void AppendCars ()
        {
            if (Cars.Count < Properties.MaximumCars)
            {
                CreateCar ();
            }
        }

        //------------------------------------------------------------------
        private void CleanUp ()
        {
            // Remove Cars outside the screen
            var border = Properties.Height * 3;
            Cars.RemoveAll (car =>
            {
                int position = (int) car.GlobalPosition.Y;
                return position < -border || position > border;
            });

            // Remove all dead Cars
            Cars.RemoveAll (car => car.Deleted);
        }

        //------------------------------------------------------------------
        private void AddQueuedCars ()
        {
            Cars.AddRange (Properties.CarsToAdd);
            Properties.CarsToAdd.ForEach (OwnCar);
            Properties.CarsToAdd.Clear ();
        }

        //------------------------------------------------------------------
        public void Add (Car car)
        {
            Properties.CarsToAdd.Add (car);
        }

        //------------------------------------------------------------------
        private void OwnCar (Car car)
        {
            if (car.Lane != this)
                car.Lane.Cars.Remove (car);

            car.Lane = this;
            car.Position = new Vector2 (0, car.GlobalPosition.Y);
        }

        //------------------------------------------------------------------
        private void FreeCar (Car car)
        {
//            car.Lane = null;
        }

        #endregion

        //------------------------------------------------------------------
        public override string ToString ()
        {
            return string.Format ("{0}", Properties.ID);
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
//            new Text (ToString () + ":" + Cars.Count, Position);
//            new Text (Velocity.ToString ("F0"), Position);

            int number = 1;
            foreach (var car in Cars)
            {
//                new Text (car.ID.ToString (), new Vector2 (Position.X, 20 * number));
                number++;
            }
        }
    }
}