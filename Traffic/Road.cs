using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;

namespace Traffic
{
    internal class Road
    {
        //------------------------------------------------------------------
        private List <Lane> lanes;
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
        public void Create ()
        {
            Images = Game.Content.LoadContentFolder <Texture2D> ("Images/Road");
            texture = Images["Road"];

            CreateLanes ();
            Player = lanes.First ().CreatePlayer (Game);
        }

        //------------------------------------------------------------------
        private void CreateLanes ()
        {
            lanes = new List <Lane> ();

            foreach (var index in Enumerable.Range (0, 12))
            {
                var lane = new Lane (this, index);

                if (index != 0)
                {
                    lane.Left = lanes[index - 1];
                    lane.Left.Right = lane;
                }

                lane.Create ();
                lanes.Add (lane);
            }
        }

        //------------------------------------------------------------------
        public void Update ()
        {
            foreach (var lane in lanes)
            {
                lane.Update ();
            }

            // Camera movement simulation
            MoveCamera (Player.Velocity);
        }

        //------------------------------------------------------------------
        private void MoveCamera (float shift)
        {
            // Simulate of Camera movement by moving Road
            position.Y += shift * 1.5f / Car.VelocityFactor;

            // Infinite loop for Road Texture
            if (position.Y > 800)
                position.Y = 0;

            foreach (var lane in lanes)
            {
                lane.MoveCars (shift);
            }
        }

        //------------------------------------------------------------------
        public void Draw ()
        {
            spriteBatch.Begin ();
            spriteBatch.Draw (texture, position, Color.White);
            spriteBatch.Draw (texture, position - new Vector2 (0, texture.Height), Color.White);

            foreach (var lane in lanes)
            {
                lane.Draw (spriteBatch);
            }

            spriteBatch.End ();
        }
    }
}