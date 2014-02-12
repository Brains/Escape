using System;
using System.Collections.Generic;
using System.Linq;
using Fluid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;
using Tools.Extensions;
using Traffic.Cars;

namespace Traffic
{
    public class Road : Object
    {
        public const int LanesQuantity = 12;

        //------------------------------------------------------------------
        private List <Lane> lanes;
        private Texture2D texture;
        public RenderTarget2D Obstacles { get; private set; }

        //------------------------------------------------------------------
        public Game Game { get; set; }
        public Player Player { get; set; }
        public Dictionary <string, Texture2D> Images { get; set; }

        //------------------------------------------------------------------
        public Solver Fluid { get; set; }

        //------------------------------------------------------------------
        public Road (Game game) : base (null)
        {
            Game = game;
            CreateLanes ();
            Add (new Indicators (this));

            Obstacles = new RenderTarget2D (Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        }

        //------------------------------------------------------------------
        public override void Setup()
        {
            Images = Game.Content.LoadContentFolder <Texture2D> ("Images/Road");
            texture = Images["Road"];

            Player = ((Lane) Components[6]).CreatePlayer (Game);

            base.Setup();
        }

        //------------------------------------------------------------------
        private void CreateLanes()
        {
            lanes = new List<Lane> ();
            Lane left = null;

            foreach (var index in Enumerable.Range (0, LanesQuantity))
            {
                Lane lane = new Lane (this, index);
                lane.CarsQuantity = Lane.Random.Next (Lane.MinimumCars, Lane.MaximumCars);

                // Set Lane's neiborhoods
                if (index != 0 && left != null)
                {
                    lane.Left = left;
                    lane.Left.Right = lane;
                }

                Add (lane);
                lanes.Add (lane);

                left = lane;
            }
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            // Camera movement simulation
            MoveCamera (Player.Velocity * elapsed * 2);

        }

        //------------------------------------------------------------------
        private void MoveCamera (float shift)
        {
            // Simulate of Camera movement by moving Road
            Move (shift);

            // Infinite loop for Road Texture
            if (LocalPosition.Y > 800)
                LocalPosition = new Vector2 (LocalPosition.X, 0);
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Begin (SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            base.Draw (spriteBatch);

            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public void DrawRoad (SpriteBatch spriteBatch)
        {
            spriteBatch.Begin ();

            Vector2 shift = new Vector2 (0, texture.Height);
            spriteBatch.Draw (texture, Position, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw (texture, Position - shift, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.End ();
        }

        //------------------------------------------------------------------
        //Render only Cars Textures for Fluid obstacles
        public void GenerateFluidObstacles(SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.SetRenderTarget (Obstacles);
            Game.GraphicsDevice.Clear (Color.Transparent);

            spriteBatch.Begin (SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (var lane in lanes)
                foreach (var car in lane.Cars)
                    spriteBatch.Draw (car.Texture, car.Position, null, Color.White, car.Angle, car.origin, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.End ();

            Game.GraphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        public Car FindCar (Vector2 position)
        {
            foreach (var lane in lanes)
                foreach (var car in lane.Cars)
                    if (car.Bounds.Contains (position)) return car;

            return null;
        }

        //------------------------------------------------------------------
        public Car FindClosestPolice (Car punisher)
        {
            List <Car> polices = new List <Car>();

            // Find all Polices
            foreach (var lane in lanes)
                foreach (var car in lane.Cars)
                    if (car is Police) polices.Add (car);

            // Find nearest Police
            var closestPolice = polices.MinBy (police =>
            {
                var distance = police.Position - punisher.Position;
                return distance.Length();
            });

            return closestPolice;
        }
    }
}