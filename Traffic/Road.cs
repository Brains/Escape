using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools;
using Tools.Extensions;
using Traffic.Cars;

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
            CreateLanes ();
            Add (new Indicators (this));
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            Images = Game.Content.LoadContentFolder <Texture2D> ("Images/Road");
            texture = Images["Road"];

            Player = ((Lane) Components.First ()).CreatePlayer (Game);
            ((Lane) Components[10]).CreatePolice (Game);

            base.Setup ();
        }

        //------------------------------------------------------------------
        private void CreateLanes ()
        {
            Lane left = null;

            foreach (var index in Enumerable.Range (0, 12))
            {
                Lane lane = new Lane (this, index);

                // Set Lane's neiborhoods
                if (index != 0 && left != null)
                {
                    lane.Left = left;
                    lane.Left.Right = lane;
                }

                Add (lane);

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
            if (Position.Y > 800)
                Position = new Vector2 (Position.X, 0);
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            base.Draw (spriteBatch);

            spriteBatch.Draw (texture, GlobalPosition, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw (texture, GlobalPosition - new Vector2 (0, texture.Height), null, 
                Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}