using System;
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
        public override void Setup ()
        {
            Images = Game.Content.LoadContentFolder <Texture2D> ("Images/Road");
            texture = Images["Road"];

            CreateLanes ();
            Player = ((Lane) Components.First ()).CreatePlayer (Game);
        }

        //------------------------------------------------------------------
        private void CreateLanes ()
        {
            Lane left = null;

            foreach (var index in Enumerable.Range (0, 12))
            {
                var lane = new Lane (this, index);

                if (index != 0 && left != null)
                {
                    lane.Left = left;
                    lane.Left.Right = lane;
                }

                lane.Create ();
                Add (lane);

                left = lane;
            }
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            // Camera movement simulation
            MoveCamera (Player.Velocity);
        }

        //------------------------------------------------------------------
        private void MoveCamera (float shift)
        {
            // Simulate of Camera movement by moving Road
            Move (shift);

            // Infinite loop for Road Texture
            if (Position.Y > 800)
                Position = new Vector2 (Position.X, 0);
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (texture, GlobalPosition, Color.White);
            spriteBatch.Draw (texture, GlobalPosition - new Vector2 (0, texture.Height), Color.White);

            base.Draw (spriteBatch);
        }
    }
}