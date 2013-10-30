using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;

namespace Traffic
{
    internal class Lane
    {
        private class Attributes
        {
            public int ID;
            public int MaximumCars = 6;
            public int Height;
            public List<Car> CarsToAdd;
            public List<Car> CarsToRemove;
        }

        Attributes Properties = new Attributes ();

        //------------------------------------------------------------------
        public List <Car> Cars { get; private set; }
        public Vector2 Position { get; set; }
        public int Velocity { get; set; }
        public Lane Left { get; set; }
        public Lane Right { get; set; }
        public Road Road { get; set; }
        public static Random Random { get; set; }


        #region Creation

        //------------------------------------------------------------------
        static Lane ()
        {
            Random = new Random ();
        }

        //------------------------------------------------------------------
        public Lane (Road road, int id)
        {
            Properties.ID = id;
            CalculatePosition (Properties.ID);
            CalculateVelocity (Properties.ID);
            Road = road;
            Properties.Height = Road.Game.GraphicsDevice.Viewport.Height;

            Cars = new List <Car> ();
            Properties.CarsToAdd = new List<Car> ();
            Properties.CarsToRemove = new List<Car> ();
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
        public void Create ()
        {
            int carsAmount = Random.Next (Properties.MaximumCars);

            foreach (var number in Enumerable.Range (0, carsAmount))
            {
                CreateCar ();
            }
        }

        //------------------------------------------------------------------
        private void CreateCar ()
        {
            var car = new Car (this, GetInsertionPosition ());
            car.Create ();
            Cars.Add (car);
        }

        //------------------------------------------------------------------
        public Player CreatePlayer (Game game)
        {
            var player = new Player (this, 600);
            player.Create ();
            Cars.Add (player);

            return player;
        }

        //------------------------------------------------------------------
        // Return point outside the screen
        private int GetInsertionPosition ()
        {
            float playerVelocity = (Road.Player != null ) ? Road.Player.Velocity : 0;
            int shift = (Velocity > playerVelocity) ? Properties.Height : -Properties.Height;

            return (int) (Random.NextDouble () * Properties.Height + shift);
        }

        #endregion


        #region Update

        //------------------------------------------------------------------
        public void Update ()
        {
            Cars.ForEach (car => car.Update ());

            AddQueuedCars ();
            RemoveQueuedCars ();

            CleanUpCars ();
            AppendCars ();

            new Text (ToString () + ":" + Cars.Count, Position);
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
        private void CleanUpCars ()
        {
            // Remove Cars outside the screen
            var border = Properties.Height * 3;
            Cars.RemoveAll (car =>
            {
                int position = (int) car.Position.Y;
                return position < -border || position > border;
            });
        }

        //------------------------------------------------------------------
        private void AddQueuedCars ()
        {
            Cars.AddRange (Properties.CarsToAdd);
            Properties.CarsToAdd.ForEach (OwnCar);
            Properties.CarsToAdd.Clear ();
        }

        //------------------------------------------------------------------
        private void RemoveQueuedCars ()
        {
            Properties.CarsToRemove.ForEach (FreeCar);
            Cars.RemoveAll (Properties.CarsToRemove.Contains);
            Properties.CarsToRemove.Clear ();
        }

        //------------------------------------------------------------------
        public void Add (Car car)
        {
            Properties.CarsToAdd.Add (car);
        }

        //------------------------------------------------------------------
        public void Remove (Car car)
        {
            Properties.CarsToRemove.Add (car);
        }

        //------------------------------------------------------------------
        private void OwnCar (Car car)
        {
            car.Lane = this;
            car.Position = new Vector2 (Position.X, car.Position.Y);
        }

        //------------------------------------------------------------------
        private void FreeCar (Car car)
        {
//            car.Lane = null;
        }

        #endregion


        #endregion

        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            foreach (var car in Cars)
            {
                car.Draw (spriteBatch);
            }
        }


        //------------------------------------------------------------------
        public override string ToString ()
        {
            return string.Format ("{0}", Properties.ID);
        }

        //------------------------------------------------------------------
        public bool IsFreeSpace (float horizontal, float height)
        {
            // ToDo: Use GetClosestCar
            // ToDo: I can use Rectangle.Intersection here

            foreach (var car in Cars)
            {
                float upperBorder = horizontal - (height + car.Height) / 1;
                float lowerBorder = horizontal + (height + car.Height) / 1;

                new Tools.Markers.Rectangle (
                    new Vector2 (Position.X - 20, upperBorder + height / 2),
                    new Vector2 (Position.X + 20, lowerBorder - height / 2), Color.LightGray);

                if (car.Position.Y > upperBorder && car.Position.Y < lowerBorder)
                    return false;
            }

            return true;
        }

        //------------------------------------------------------------------
        public void MoveCars (float shift)
        {
            foreach (var car in Cars)
            {
                car.Move (shift);
            }
        }
    }
}