using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;
using Tools.Extensions;
using Traffic.Cars;

namespace Traffic
{
    public class Road : Engine.Object
    {
        public const int LanesQuantity = 12;

        //------------------------------------------------------------------
        private List <Lane> lanes;

        //------------------------------------------------------------------
        public Game Game { get; set; }
        public Player Player { get; set; }
        public Dictionary <string, Texture2D> Images { get; set; }

        //------------------------------------------------------------------
        public Road (Game game) : base (null)
        {
            Game = game;

            CreateLanes();
            Add (new Indicators (this));
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            CreateDrawable (game, "Road");
            
            // To draw Road under Cars
            Drawable.Depth = 1;

            Images = game.Content.LoadFolder<Texture2D> ("Images/Road");
            Player = ((Lane) Components[6]).CreatePlayer (game);

            base.Setup (game);
        }

        //------------------------------------------------------------------
        private void CreateLanes()
        {
            lanes = new List <Lane>();
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
            MoveCamera (Player.Velocity * elapsed * 2); // 2 - ratio for simulate very high speed
        }

        //------------------------------------------------------------------
        private void MoveCamera (float shift)
        {
            // Simulate of Camera movement by moving Road
            Move (new Vector2 (0, shift));

            // Infinite loop for Road Texture
            if (LocalPosition.Y > Game.GraphicsDevice.Viewport.Height)
                LocalPosition = Vector2.Zero; //new Vector2 (LocalPosition.X, 0);
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch batch)
        {
            Vector2 shift = new Vector2 (0, -Drawable.Height);

            // Move Road to the Point[0, 0] instead of the Point[origin]
            Move (Drawable.Origin);

            // Draw Road and all her Components
            base.Draw (batch);

            // Shift up the Road and draw it again to implement continuously Road
            Move (shift);
            Drawable.Draw (batch);
            
            // Restore Position
            Move (-shift);
            Move (-Drawable.Origin);
        }

        //------------------------------------------------------------------
        public Car FindCar (Vector2 position)
        {
            // ToDo: Convert to LINQ manually
            foreach (var lane in lanes)
                foreach (var car in lane.Cars)
                    if (car.Bounds.Contains (position)) return car;

            return null;
        }

        //------------------------------------------------------------------
        public Car FindClosestPolice (Car punisher)
        {
            List <Car> polices = new List <Car>();

            // ToDo: Convert to LINQ manually
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