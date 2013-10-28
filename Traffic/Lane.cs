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
    class Lane
    {
        private int ID;
        private List <Car> carsToAdd;
        private List <Car> carsToRemove;

        //------------------------------------------------------------------
        public List <Car> Cars { get; set; }
        public Vector2 Position { get; set; }
        public float Velocity { get; set; }
        public Lane Left { get; set; }
        public Lane Right { get; set; }

        static public Random Random { get; set; }


        #region Creation

        //------------------------------------------------------------------
        static Lane ()
        {
            Random = new Random ();
        }

        //------------------------------------------------------------------
        public Lane (int id, Game game, float position, float velocity)
        {
            ID = id;
            Position = new Vector2 (position, 0);
            Velocity = velocity;
            carsToAdd = new List <Car> ();
            carsToRemove = new List <Car> ();

            Cars = new List<Car> ();
            CreateCars (game, velocity);
        }

        //------------------------------------------------------------------
        private void CreateCars (Game game, float velocity)
        {
            int carsAmount = Random.Next (5);

            foreach (var number in Enumerable.Range (0, carsAmount))
            {
                Car car = new Car (game, this);
                car.Position = new Vector2 (Position.X, (float) Random.NextDouble ()*400);
                Cars.Add (car);
            }
        }

        //------------------------------------------------------------------
        public void CreatePlayer (Game game)
        {
            var player = new Player (game, this);
            player.Position = new Vector2 (Position.X, (float) 600);
            Cars.Add (player);
        }


        //------------------------------------------------------------------
        public void Initialize ()
        {
            foreach (var car in Cars)
            {
                car.Initialize ();
            }
        }

        //------------------------------------------------------------------
        public void LoadContent ()
        {
            foreach (var car in Cars)
            {
                car.LoadContent ();
            }
        }

        #endregion


        //------------------------------------------------------------------
        public void UnloadContent ( )
        {
            foreach (var car in Cars)
            {
                car.UnloadContent ();
            }
        }


        #region Update

        //------------------------------------------------------------------
        public void Update ()
        {
            Cars.ForEach (car => car.Update ());

            AddCars ();
            RemoveCars ();
        }

        //------------------------------------------------------------------
        public void Add (Car car)
        {
            carsToAdd.Add (car);
        }

        //------------------------------------------------------------------
        private void AddCars ()
        {
            Cars.AddRange (carsToAdd);
            carsToAdd.ForEach (OwnCar);
            carsToAdd.Clear ();
        }

        //------------------------------------------------------------------
        private void OwnCar (Car car)
        {
            car.Lane = this;
            car.Position = new Vector2 (Position.X, car.Position.Y);
        }

        //------------------------------------------------------------------
        public void Remove (Car car)
        {
            carsToRemove.Add (car);
        }

        //------------------------------------------------------------------
        private void RemoveCars ()
        {
            carsToRemove.ForEach (car => car.Lane = null);
            Cars.RemoveAll (carsToRemove.Contains);
            carsToRemove.Clear ();
        }

        #endregion


        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            foreach (var car in Cars)
            {
                car.Draw (spriteBatch);
            }

            var shift = new Vector2 (20, 0);
            new Tools.Markers.Line (Position + shift, Position + shift + new Vector2 (0, 800), Color.LightSteelBlue);
        }


        //------------------------------------------------------------------
        public override string ToString ()
        {
            return string.Format ("Lane: {0}", ID);
        }

        //------------------------------------------------------------------
        public bool IsFreeSpace (float horizontal, float height)
        {
            foreach (var car in Cars)
            {
                float lowerBorder = horizontal - height / 2 - car.Height / 2;
                float upperBorder = horizontal + height / 2 + car.Height / 2;

                new Tools.Markers.Rectangle (
                    new Vector2 (Position.X - 20, lowerBorder + height / 2),
                    new Vector2 (Position.X + 20, upperBorder - height / 2), Color.Red);


                if (car.Position.Y > lowerBorder && car.Position.Y < upperBorder)
                    return false;
            }

            return true;
        }
    }
}