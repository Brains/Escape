using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    class Lane
    {
        List <Car> cars;

        static public Random Random { get; set; }
        public float Position { get; set; }
        public float Velocity { get; set; }

        //------------------------------------------------------------------
        static Lane ()
        {
            Random = new Random();
        }

        //------------------------------------------------------------------
        public Lane (Game game)
        {
            CreateCars (game);
        }

        //------------------------------------------------------------------
        private void CreateCars (Game game)
        {
            int carsAmount = 1/*Random.Next (5)*/;
            cars = new List <Car> ();

            foreach (var number in Enumerable.Range (0, carsAmount))
            {
                Car car = new Car (game, this);
                car.VerticalPosition = 300/*(float) Random.NextDouble () * 800*/;
                cars.Add (car);
            }
        }

        //------------------------------------------------------------------
        public void Initialize ()
        {
            foreach (var car in cars)
            {
                car.Initialize ();
            }
        }

        //------------------------------------------------------------------
        public void LoadContent ( )
        {
            foreach (var car in cars)
            {
                car.LoadContent ();
            }
        }

        //------------------------------------------------------------------
        public void UnloadContent ( )
        {
            foreach (var car in cars)
            {
                car.UnloadContent ();
            }
        }

        //------------------------------------------------------------------
        public void Update ( )
        {
            foreach (var car in cars)
            {
                car.Update ();
            }
        }

        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            foreach (var car in cars)
            {
                car.Draw (spriteBatch);
            }
        }

    }
}