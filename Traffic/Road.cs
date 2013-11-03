using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;
using Tools.Extensions;

namespace Traffic
{
    internal class Road : Object
    {
        //------------------------------------------------------------------
//        private List <Lane> Components;
        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Vector2 position;

        //------------------------------------------------------------------
        public Game Game { get; set; }
        public Player Player { get; set; }
        public Dictionary <string, Texture2D> Images { get; set; }

        //------------------------------------------------------------------
        public Road (Game game)
        {
            Game = game;

            spriteBatch = new SpriteBatch (Game.GraphicsDevice);
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            Images = Game.Content.LoadContentFolder <Texture2D> ("Images/Road");
            texture = Images["Road"];

            CreateLanes ();
            Player = Components.First ().CreatePlayer (Game);
        }

        //------------------------------------------------------------------
        private void CreateLanes ()
        {
//            Components = new List <Lane> ();

            foreach (var index in Enumerable.Range (0, 12))
            {
                var lane = new Lane (this, index);

                if (index != 0)
                {
                    lane.Left = Components[index - 1];
                    lane.Left.Right = lane;
                }

                lane.Create ();
                Components.Add (lane);
            }
        }

        //------------------------------------------------------------------
        public override void Update ()
        {
            foreach (var lane in Components)
                lane.Update ();

            // Camera movement simulation
            MoveCamera (Player.Velocity);
        }

        //------------------------------------------------------------------
        private void MoveCamera (float shift)
        {
            // Simulate of Camera movement by moving Road
            position.Y += shift * 2.0f / Car.VelocityFactor;

            // Infinite loop for Road Texture
            if (position.Y > 800)
                position.Y = 0;

            foreach (var lane in Components)
                lane.MoveCars (shift);
        }

        //------------------------------------------------------------------
        public override void Draw ()
        {
            spriteBatch.Begin ();
            spriteBatch.Draw (texture, position, Color.White);
            spriteBatch.Draw (texture, position - new Vector2 (0, texture.Height), Color.White);

            foreach (var lane in Components)
                lane.Draw (spriteBatch);

            spriteBatch.End ();
        }
    }
}