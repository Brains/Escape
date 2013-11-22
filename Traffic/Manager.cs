using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Actions;
using Traffic.Cars;

namespace Traffic
{
    public class Manager : DrawableGameComponent
    {
        private readonly Road road;
        private SpriteBatch spriteBatch;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            road = new Road (Game);
        }

        //------------------------------------------------------------------
        public override void Initialize ()
        {
            spriteBatch = new SpriteBatch (Game.GraphicsDevice);
            
            road.Setup ();

            CreateEvents ();
        }

        //------------------------------------------------------------------
        private void CreateEvents ()
        {
            Tools.Timers.Loop.Create (1, ChangeMaximumCarsOnLaneEvent);
            Tools.Timers.Loop.Create (1, ChangeLaneForCarEvent);
//            Tools.Timers.Loop.Create (3, CreatePolice);
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            road.Update (elapsed);
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            spriteBatch.Begin (SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            road.Draw (spriteBatch);

            spriteBatch.End ();

        }

        //-----------------------------------------------------------------
        private void ChangeLaneForCarEvent ( )
        {
            // To Left
            var car = GetRandomCar ();
            car.Driver.AddInSequnce (new TryChangeLane (car.Driver, car.Lane.Left));
            
            // To Right
            car = GetRandomCar ();
            car.Driver.AddInSequnce (new TryChangeLane (car.Driver, car.Lane.Left));
        }

        //------------------------------------------------------------------
        private Car GetRandomCar ()
        {
            var laneID = Lane.Random.Next(12);
            Lane lane = (Lane) road.Components[laneID];

            var carID = Lane.Random.Next (lane.MaximumCars);
            if (carID >= lane.Cars.Count) carID = lane.Cars.Count - 1;
            var car = lane.Cars[carID];

            return car;
        }

        //------------------------------------------------------------------
        private void ChangeMaximumCarsOnLaneEvent ()
        {
            var laneID = Lane.Random.Next (12);
            Lane lane = (Lane) road.Components[laneID];
            lane.MaximumCars = Lane.Random.Next (5, 25);
        }

        //-----------------------------------------------------------------
        private void CreatePolice ( )
        {
            var laneID = Lane.Random.Next (12);
            Lane lane = (Lane) road.Components[laneID];
            
            lane.CreatePolice (Game);
        }
    }
}
