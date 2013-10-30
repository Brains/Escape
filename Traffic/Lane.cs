using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;
using Tools.Markers;

namespace Traffic
{
    internal class Lane
    {
        private int ID;
        private List <Car> carsToAdd;
        private List <Car> carsToRemove;
        private int maximumCars = 4;
        private int height;

        // ToDo: Nested Properties (Try, Test, Implement everywhre, StackOverflow)
        private class Properties
        {
        }

        private class Helpers
        {
        }

        //------------------------------------------------------------------
        public List <Car> Cars { get; set; }
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
            ID = id;
            CalculatePosition (ID);
            CalculateVelocity (ID);
            Road = road;
            height = Road.Game.GraphicsDevice.Viewport.Height;

            Cars = new List <Car> ();
            carsToAdd = new List <Car> ();
            carsToRemove = new List <Car> ();
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
            int carsAmount = Random.Next (maximumCars);

            foreach (var number in Enumerable.Range (0, carsAmount))
            {
                CreateCar ();
            }
        }

        //------------------------------------------------------------------
        private void CreateCar ()
        {
            var car = new Car (this, GetInsertPosition ());
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
        private int GetInsertPosition ()
        {
            float playerVelocity = (Road.Player != null ) ? Road.Player.Velocity : 0;
            int shift = (Velocity > playerVelocity) ? height : -height;

            return (int) (Random.NextDouble () * height + shift);
        }

        #endregion

        #region Update

        //------------------------------------------------------------------
        public void Update ()
        {
            Cars.ForEach (car => car.Update ());

            // ToDo: ѕотому что вначале добавл€ем а затем Lane = null указатель на Lane == null?
            AddQueuedCars ();
            RemoveQueuedCars ();

            CleanUpCars ();
            AppendCars ();

            new Text (Cars.Count.ToString (), Position);
        }


        #region Cars Management

        //------------------------------------------------------------------
        private void AppendCars ()
        {
            if (Cars.Count < maximumCars)
            {
                CreateCar ();
            }
        }

        //------------------------------------------------------------------
        private void CleanUpCars ()
        {
            // Remove Cars outside the screen
            var border = height * 3;
            Cars.RemoveAll (car =>
            {
                int position = (int) car.Position.Y;
                return position < -border || position > border;
            });
        }

        //------------------------------------------------------------------
        private void AddQueuedCars ()
        {
            Cars.AddRange (carsToAdd);
            carsToAdd.ForEach (OwnCar);
            carsToAdd.Clear ();
        }

        //------------------------------------------------------------------
        private void RemoveQueuedCars ()
        {
            carsToRemove.ForEach (FreeCar);
            Cars.RemoveAll (carsToRemove.Contains);
            carsToRemove.Clear ();
        }

        //------------------------------------------------------------------
        public void Add (Car car)
        {
            carsToAdd.Add (car);
        }

        //------------------------------------------------------------------
        public void Remove (Car car)
        {
            carsToRemove.Add (car);
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
            car.Lane = null;
        }

        #endregion

        //------------------------------------------------------------------

        //------------------------------------------------------------------

        //------------------------------------------------------------------

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
            return string.Format ("Lane: {0}", ID);
        }

        //------------------------------------------------------------------
        public bool IsFreeSpace (float horizontal, float height)
        {
            // ToDo: Use GetClosestCar

            foreach (var car in Cars)
            {
                float lowerBorder = horizontal - (height + car.Height) / 2;
                float upperBorder = horizontal + (height + car.Height) / 2;

//                new Tools.Markers.Rectangle (
//                    new Vector2 (Position.X - 20, lowerBorder + height / 2),
//                    new Vector2 (Position.X + 20, upperBorder - height / 2), Color.Red);

                if (car.Position.Y > lowerBorder && car.Position.Y < upperBorder)
                    return false;
            }

            return true;
        }

        //------------------------------------------------------------------
        public void MoveCars ()
        {
            // Move all Cars except Player
            foreach (var car in Cars)
            {
                if (car == Road.Player) continue;

                car.Move (car.Velocity);
            }
        }
    }
}