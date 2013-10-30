using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;
using Tools.Markers;
using Tools;

namespace Traffic
{
    class Player : Car
    {
        private int maximumSpeed = 500;
        private int minimumSpeed = 50;
        private float acceleration = 0.02f;

        //------------------------------------------------------------------
        public Player (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            InitialColor = Color.SkyBlue;
            TextureName = "Player";
        }

        #region Update

        //------------------------------------------------------------------
        public override void Update ()
        {
            base.Update ();

            UpdateInput ();

            UpdateSensor ();
            
            DetectCollisions ();

            new Text (Velocity.ToString ("F0"), Position, Color.Maroon, true);
        }

        //------------------------------------------------------------------
        private void UpdateInput ()
        {
            if (KeyboardInput.IsKeyPressed (Keys.Right)) ChangeOnRightLane ();
            if (KeyboardInput.IsKeyPressed (Keys.Left)) ChangeOnLeftLane ();
            if (KeyboardInput.IsKeyDown (Keys.Down)) Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) Accelerate (); //Velocity += Velocity * 0.02f;
        }

        #endregion


        //------------------------------------------------------------------
        private void AvoidCollisions ()
        {
            if (ChangeOnLeftLane ()) return;
            if (ChangeOnRightLane()) return;
            Brake ();
        }

        //------------------------------------------------------------------
        private void UpdateSensor ()
        {
            float distance;
            var closestCar = GetClosestCarAhead (out distance);

            if (closestCar == null) return;

            float dangerousZone = (Height + closestCar.Height) / 1.0f;

            if (distance < dangerousZone)
            {
                Color = Color.Maroon;

                AvoidCollisions ();
            }
        }

        // ToDo: Refactor all this ugly method (MinBy(int instead float and then int==int)
        //------------------------------------------------------------------
        private Car GetClosestCarAhead (out float minimumDistance)
        {
            Car closestCar = Lane.Cars.MinBy (car =>
            {
                // Don't react with myself
                if (car == this) return float.MaxValue;

                var distance = Position - car.Position;

                // If the car is behind
                if (distance.Y < 0) return float.MaxValue;

                return distance.Y;
            });

            minimumDistance = Position.Y - closestCar.Position.Y;

            if (closestCar == this) return null;

            // If closest car is behind
            if (minimumDistance < 0) return null;

            closestCar.Color = Color.DarkOrange;

            return closestCar;
        }

        #region Collisions Detection

        //------------------------------------------------------------------
        private void DetectCollisions ()
        {
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

        #endregion


        #region Controls

        //------------------------------------------------------------------
        private void Accelerate ()
        {
            if (Velocity < maximumSpeed)
                Velocity += (Velocity * acceleration);

        }

        //------------------------------------------------------------------
        private void Brake ()
        {
            if (Velocity > minimumSpeed)
                Velocity -= (Velocity * acceleration);
        }

        #endregion
    }
}