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
            Color = Color.SkyBlue;
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

//            DetectCollisions ();

//            Position += new Vector2 (0, -Velocity / 100);

            UpdateSensor ();


            base.Update ();
        }

        //------------------------------------------------------------------
        private void UpdateSensor ()
        {
            Color = Color.SkyBlue;

            foreach (var car in Lane.Cars)
            {
                if (car == this) continue;

                var distance = Position - car.Position;

                // ToDo I need to find Minimal distance and compare it with treshold

                bool collisionDanger = distance.Y < (Height + car.Height) / 1.5f;
                bool isBehind = distance.Y > 0;

                if (collisionDanger && isBehind)
                {
                    new Text (distance.Y.ToString (), car.Position, Color.Maroon, true);
                    Color = Color.Red;
                    
                    AvoidCollisions ();
                    break;
                }
            }
        }

        //------------------------------------------------------------------
        private void AvoidCollisions ()
        {
            if (ChangeOnLeftLane ()) return;
            if (ChangeOnRightLane()) return;
            Brake ();
        }

        //------------------------------------------------------------------
        private void Brake ()
        {
            Velocity -= Velocity * 0.3f;
            new Text ("Brake", Position, Color.Maroon, true);

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