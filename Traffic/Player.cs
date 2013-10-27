using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;
using Tools.Markers;

namespace Traffic
{
    class Player : Car
    {
        private Game game;

        //------------------------------------------------------------------
        public Player (Game game, Lane lane) : base (game, lane)
        {
            this.game = game;
        }

        //------------------------------------------------------------------
        public override void LoadContent ()
        {
            base.LoadContent ();

            texture = game.Content.Load <Texture2D> ("Images/Cars/Player");
        }

        //------------------------------------------------------------------
        public override void Update ()
        {
            UpdateInput ();

            DetectCollisions ();

            base.Update ();
        }

        //------------------------------------------------------------------
        private void UpdateInput ()
        {
            var shift = Vector2.Zero;

            if (KeyboardInput.IsKeyPressed (Keys.Right)) ChangeOnRightLane ();
            if (KeyboardInput.IsKeyPressed (Keys.Left)) ChangeOnLeftLane ();
            if (KeyboardInput.IsKeyDown (Keys.Down)) shift.Y++;
            if (KeyboardInput.IsKeyDown (Keys.Up)) shift.Y--;

            Position += shift;
        }

        //------------------------------------------------------------------
        private void DetectCollisions ()
        {
            Color = Color.White;

            DetectCollisionsOnLane (Lane.Left);
            DetectCollisionsOnLane (Lane);
            DetectCollisionsOnLane (Lane.Right);
        }

        //------------------------------------------------------------------
        private void DetectCollisionsOnLane (Lane lane)
        {
            if (lane == null) return;

            if (lane.Cars.Any (Intersect))
                Color = Color.OrangeRed;
        }
    }
}