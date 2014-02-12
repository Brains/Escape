using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Cars;
using Traffic.Cars.Weights;

namespace Traffic
{
    public class Lane : Object
    {
        private int height;
        private readonly List <Car> carsToAdd;
        private static int carsCounter;

        //------------------------------------------------------------------
        public const int MinimumCars = 3;
        public const int MaximumCars = Settings.MaximumCarsOnLane;

        //------------------------------------------------------------------
        public readonly int ID;
        private int border;
        public List <Car> Cars { get; private set; }
        public int Velocity { get; set; }
        public Lane Left { get; set; }
        public Lane Right { get; set; }
        public static Random Random { get; set; }
        public Road Road { get; private set; }
        public int CarsQuantity { get; set; }

        #region Creation

        //------------------------------------------------------------------
        static Lane ()
        {
            Random = new Random ();
        }

        //------------------------------------------------------------------
        public Lane (Road road, int id) : base (road)
        {
            ID = id;
            Road = road;
            Anchored = true;

            CalculatePosition (ID);
            CalculateVelocity (ID);

            Cars = new List <Car> ();
            carsToAdd = new List <Car> ();
        }

        //------------------------------------------------------------------
        private void CalculateVelocity (int id)
        {
            const int maximumVelocity = 240;
            const int step = 20;
            Velocity = maximumVelocity - id * step;
        }

        //------------------------------------------------------------------
        private void CalculatePosition (int id)
        {
            const int laneWidth = 40;
            int position = id * laneWidth + laneWidth / 2;

            LocalPosition = new Vector2 (position, 0);
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            height = Road.Game.GraphicsDevice.Viewport.Height;
            border = 800;

            base.Setup ();
        }

        //------------------------------------------------------------------
        protected virtual Car CreateCar ()
        {
            var weight = GetWeight ();
            var textureName = "Car " + weight.TextureSuffix;

            var car = new Car (this, carsCounter, GetInsertionPosition (), weight, textureName);
            car.Setup ();

            Cars.Add (car);
            OwnCar (car);

            carsCounter++;

            return car;
        }

        //-----------------------------------------------------------------
//        private void CreateCar (int position)
//        {
//            var car = new Car (this, carsCounter, position, new Heavy (), "Car (Heavy)");
//            car.Setup ();
//
//            Cars.Add (car);
//            OwnCar (car);
//                 
//        }

        //------------------------------------------------------------------
        private Weight GetWeight ()
        {
            if (ID >= 0 && ID < 4)
                return new Light ();
            if (ID >= 4 && ID < 8)
                return new Medium ();
            if (ID >= 8 && ID < 12)
                return new Heavy ();

            return null;
        }

        //------------------------------------------------------------------
        public Player CreatePlayer (Game game)
        {
            var player = new Player (this, carsCounter, 400, GetWeight (), "Player");
            player.Setup ();

            Cars.Add (player);
            OwnCar (player);

            carsCounter++;

//            Left.CreateCar (300);
//            Left.CreateCar (420);
//            Left.Left.Left.CreateCar (400);

            return player;
        }

        //------------------------------------------------------------------
        public Police CreatePolice (Game game)
        {
            var police = new Police (this, carsCounter, Settings.PoliceStartPosition, GetWeight (), "Police");
            police.Setup ();

            Cars.Add (police);
            OwnCar (police);

            carsCounter++;

            return police;
        }

        //------------------------------------------------------------------
        // Return point outside the screen
        private int GetInsertionPosition ()
        {
            // Determine where place car: above Player or bottom
            float playerVelocity = (Road.Player != null) ? Road.Player.Velocity : 0;

            int sign = (Velocity > playerVelocity) ? 1 : -1;

            return GetFreePosition (sign);
        }

        //------------------------------------------------------------------
        private int GetFreePosition (int sign)
        {
            int position = 0;
            int lower = 0 + sign * border;
            int upper = height + sign * border;

            // Get free position for 20 iterations
            foreach (var index in Enumerable.Range (0, 20))
            {
//                if (index == 19) Console.WriteLine ("No Space");
                    
                position = Random.Next (lower, upper);

                if (!Cars.Any ()) break;

                float minimum = Cars.Min (car => Math.Abs (car.Position.Y - position));

                if (minimum > 150) break;
            }

            return position;
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
            if (Settings.NoCars) return;

            if (Cars.Count < CarsQuantity)
                CreateCar ();
        }

        //------------------------------------------------------------------
        private void CleanUp ()
        {
            // Remove Cars outside the screen
            Cars.RemoveAll (car =>
            {
                int position = (int) car.Position.Y;
                return position < -border || position > height + border;
            });

            // Remove all dead Cars
            Cars.RemoveAll (car => car.Deleted);
        }

        //------------------------------------------------------------------
        private void AddQueuedCars ()
        {
            Cars.AddRange (carsToAdd);
            carsToAdd.ForEach (OwnCar);
            carsToAdd.Clear ();
        }

        //------------------------------------------------------------------
        public void Add (Car car)
        {
            carsToAdd.Add (car);
        }

        //------------------------------------------------------------------
        private void OwnCar (Car car)
        {
            if (car.Lane != this)
                car.Lane.Cars.Remove (car);

            car.LocalPosition = new Vector2 (car.Position.X - Position.X, car.Position.Y);
            car.Lane = this;
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
            return string.Format ("{0}", ID);
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
//            new Text (ToString(), LocalPosition, Color.Orange);
//            new Text (Velocity.ToString ("F0"), LocalPosition);
//            new Text (CarsQuantity.ToString (), LocalPosition);

//            // Particular Type counter
//            int number = Cars.OfType <Player> ().Count ();
//            if (number != 0) 
//                new Text (number.ToString (""), LocalPosition);
        }
    }
}