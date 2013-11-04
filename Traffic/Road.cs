using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;
using Tools.Extensions;

namespace Traffic
{
    internal class Road
    {
        //------------------------------------------------------------------
<<<<<<< HEAD
=======
        private List <Lane> lanes;
        private SpriteBatch spriteBatch;
>>>>>>> parent of 62fdd8c... Object (Composite): Start
        private Texture2D texture;

        //------------------------------------------------------------------
        public Game Game { get; set; }
        public Player Player { get; set; }
        public Dictionary <string, Texture2D> Images { get; set; }

        //------------------------------------------------------------------
        public Road (Game game) : base (null)
        {
            Game = game;
        }

        //------------------------------------------------------------------
        public void Create ()
        {
            Images = Game.Content.LoadContentFolder <Texture2D> ("Images/Road");
            texture = Images["Road"];

            CreateLanes ();
<<<<<<< HEAD
            Player = (Components.First () as Lane).CreatePlayer (Game);
=======
            Player = lanes.First ().CreatePlayer (Game);
>>>>>>> parent of 62fdd8c... Object (Composite): Start
        }

        //------------------------------------------------------------------
        private void CreateLanes ()
        {
<<<<<<< HEAD
            Lane left = null;
=======
            lanes = new List <Lane> ();
>>>>>>> parent of 62fdd8c... Object (Composite): Start

            foreach (var index in Enumerable.Range (0, 12))
            {
                var lane = new Lane (this, index);

                if (index != 0)
                {
<<<<<<< HEAD
                    lane.Left = left;
=======
                    lane.Left = lanes[index - 1];
>>>>>>> parent of 62fdd8c... Object (Composite): Start
                    lane.Left.Right = lane;
                }

                left = lane;

                lane.Create ();
<<<<<<< HEAD
                Add (lane);
=======
                lanes.Add (lane);
>>>>>>> parent of 62fdd8c... Object (Composite): Start
            }
        }

        //------------------------------------------------------------------
<<<<<<< HEAD
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

=======
        public void Update ()
        {
            foreach (var lane in lanes)
                lane.Update ();
>>>>>>> parent of 62fdd8c... Object (Composite): Start

            // Camera movement simulation
            MoveCamera (Player.Velocity);
        }

        //------------------------------------------------------------------
        private void MoveCamera (float shift)
        {
            // Simulate of Camera movement by moving Road
            Position += new Vector2 (0, shift / Car.VelocityFactor);

            // Infinite loop for Road Texture
<<<<<<< HEAD
            if (Position.Y > 800)
                Position = new Vector2 (Position.X, 0);
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (texture, Position, Color.White);
            spriteBatch.Draw (texture, Position - new Vector2 (0, texture.Height), Color.White);
            
            base.Draw (spriteBatch);
=======
            if (position.Y > 800)
                position.Y = 0;

            foreach (var lane in lanes)
                lane.MoveCars (shift);
        }

        //------------------------------------------------------------------
        public void Draw ()
        {
            spriteBatch.Begin ();
            spriteBatch.Draw (texture, position, Color.White);
            spriteBatch.Draw (texture, position - new Vector2 (0, texture.Height), Color.White);

            foreach (var lane in lanes)
                lane.Draw (spriteBatch);

            spriteBatch.End ();
>>>>>>> parent of 62fdd8c... Object (Composite): Start
        }
    }
}