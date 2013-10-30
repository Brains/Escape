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
            UpdateInput ();

            UpdateSensor ();
            
            DetectCollisions ();

            base.Update ();
            
            new Text (Lane.ToString (), Position, Color.Red, true);
        }

        //------------------------------------------------------------------
        public void UpdateInput ()
        {
            if (KeyboardInput.IsKeyPressed (Keys.Right)) ChangeOnRightLane ();
            if (KeyboardInput.IsKeyPressed (Keys.Left)) ChangeOnLeftLane ();
            if (KeyboardInput.IsKeyDown (Keys.Down)) Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) Accelerate (); 
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
            var closestCar = GetClosestCarAhead ();

            if (closestCar == null) return;
            
            float distance = Distance (closestCar);

            closestCar.Color = Color.DarkOrange;

            float dangerousZone = (Height + closestCar.Height) / 1.0f;

            if (distance < dangerousZone)
            {
                Color = Color.Maroon;

//                AvoidCollisions ();
            }
        }

        //------------------------------------------------------------------
        private Car GetClosestCarAhead ()
        {
            var aheadCars = Lane.Cars.Where (car => car.Position.Y < Position.Y);
            
            Car closestCar = aheadCars.MinBy (Distance);

            return closestCar;
        }

        //------------------------------------------------------------------
        private float Distance (Car car)
        {
            // Don't react with myself
            if (car == this) return float.MaxValue;

            var distance = Position - car.Position;

            return distance.Y;
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

            // ToDo: Use GetClosestCar

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

//            Position -= new Vector2 (0, 0.5f);
        }

        //------------------------------------------------------------------
        private void Brake ()
        {
            if (Velocity > minimumSpeed)
                Velocity -= (Velocity * acceleration);

//            Position += new Vector2 (0, 0.5f);
        }

        #endregion
    }
}