using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Traffic.Actions;
using Traffic.Drivers;
using Traffic.Helpers;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Tools;
using Tools.Markers;
using Point = Tools.Markers.Point;

namespace Traffic
{
    internal class Car : Object
    {
        //-----------------------------------------------------------------
        private Lane lane;
        private float angle;
        private Vector2 origin;
        private Bounds bounds;

        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;

        //------------------------------------------------------------------
        public int ID;

        public float Velocity { get; set; }
        public Color Color { get; set; }
        public int Lenght { get; set; }
        public Driver Driver { get; set; }
        public int Lives { get; set; }
        public float Acceleration { get; set; }

        //------------------------------------------------------------------
        public Lane Lane
        {
            get { return lane; }
            set
            {
                lane = value;
                Root = value;
            }
        }

        #region Creation

        //------------------------------------------------------------------
        public Car (Lane lane, int horizont) : base (lane)
        {
            Lane = lane;
            Driver = new Common (this);
            InitialColor = Color.White;
            TextureName = "Car";
            Position = new Vector2 (0, horizont);
            Lives = 1;
            Acceleration = 3;
        }

        //------------------------------------------------------------------
        public void Create ()
        {
            Texture = Lane.Road.Images[TextureName];
            origin = new Vector2 (Texture.Width / 2, Texture.Height / 2);
            Lenght = Texture.Height;

            Driver.Create ();

            bounds = new Bounds (this, GlobalPosition, origin);
            bounds.Inflate (-5, -5);
            Add (bounds);
        }

        #endregion

        #region Update

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            Color = InitialColor;

            Move (-Velocity);

            // Simulate Camera moving
            Move (Lane.Road.Player.Velocity);

            Driver.Update (elapsed);

            DetectCollisions ();

            Debug ();
        }

        #endregion

        #region Controls

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            if (Velocity < Driver.Velocity)
                Velocity += Acceleration;
//            new Text ("Accelerate", GlobalPosition, Color.DarkOrange);
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Velocity > 0)
                Velocity -= Acceleration;
//            new Text ("Brake", GlobalPosition, Color.DarkOrange);
        }

        //------------------------------------------------------------------
        public void ChangeLane (Lane lane)
        {
            if (lane == null) return;

            AnimateGhangingLane (lane);
        }

        //------------------------------------------------------------------
        private void AnimateGhangingLane (Lane newLane)
        {
            // No Lane changing when car doesn't move
            if (Velocity < 10) return;
            
            var sequence = new Sequence {Lock = true};
            float duration = 400.0f / Velocity;

            // Rotate
            Action <float> rotate = share => angle += share;
            float finalAngle = MathHelper.ToRadians ((newLane.GlobalPosition.X < GlobalPosition.X) ? -10 : 10);
            sequence.Add (new Controller (rotate, finalAngle, duration * 0.1f));

            // Moving
            Action <Vector2> move = shift => Position += shift;
            var diapason = new Vector2 (newLane.GlobalPosition.X - GlobalPosition.X, 0);
            sequence.Add (new Controller (move, diapason, duration * 0.2f));

            // Inverse rotating
            var inverseRotating = new Controller (rotate, -finalAngle, duration * 0.1f);
            sequence.Add (inverseRotating);

            // Add to new Lane
            sequence.Add (new Generic (() => newLane.Add (this)));

            Driver.Add (sequence);
        }

        #endregion

        #region Collisions Detection

        //------------------------------------------------------------------
        protected void DetectCollisions ()
        {
            DetectCollisionsOnLane (Lane.Left);
            DetectCollisionsOnLane (Lane);
            DetectCollisionsOnLane (Lane.Right);
        }

        //------------------------------------------------------------------
        private void DetectCollisionsOnLane (Lane lane)
        {
//            if (Lives <= 0) return;
            if (lane == null) return;

            var closestCar = Driver.FindClosestCar (lane.Cars);
            if (closestCar == null) return;

            if (Intersect (closestCar))
            {
                Lives--;
//                closestCar.Lives--;
                if (this is Player)
                {
//                    Console.WriteLine (ToString () + " : " + closestCar.ID + " : " + closestCar.Lives);                        
                }
            }

            if (Lives <= 0)
                Deleted = true;

//            new Text (Lives.ToString (), Position, Color.RoyalBlue, true);
        }

        //------------------------------------------------------------------
        public bool Intersect (Car car)
        {
            if (car == this) return false;

            return bounds.Intersects (car.bounds);
        }

        #endregion

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (Texture, GlobalPosition, null, Color, angle, origin, 1.0f, SpriteEffects.None, 1.0f);

            base.Draw (spriteBatch);
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
            new Text (Velocity.ToString (), GlobalPosition, Color.GreenYellow, true);
        }
    }
}