using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Drivers;

namespace Traffic.Cars
{
    internal class Car : Object
    {
        //-----------------------------------------------------------------
        private Lane lane;
        private Vector2 origin;
        private Bounds bounds;

        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;
        private Lights brakes;
        private Lights blinker;

        //------------------------------------------------------------------
        public int ID;
        public float Velocity { get; set; }
        public Color Color { get; set; }
        public int Lenght { get; set; }
        public Driver Driver { get; set; }
        public int Lives { get; set; }
        public float Acceleration { get; set; }
        public float Angle { get; set; }

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
            Acceleration = 0.5f;
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            // Load Texture
            Texture = Lane.Road.Images[TextureName];
            origin = new Vector2 (Texture.Width / 2, Texture.Height / 2);
            Lenght = Texture.Height;

            // Bounding Box
            bounds = new Bounds (this, GlobalPosition, origin);
            bounds.Inflate (-5, -5);
            Add (bounds);

            // Lights
            brakes = new Lights (this, "Brake");
            brakes.Position = new Vector2 (0, Texture.Height / 2 + 5);
            Add (brakes);

            blinker = new Lights (this, "Blinker");
            Add (blinker);

            base.Setup ();
        }

        #endregion

        #region Update

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            Reset ();

            Move (-Velocity);

            // Simulate Camera moving
            Move (Lane.Road.Player.Velocity);

            Driver.Update (elapsed);

            DetectCollisions ();

            Debug ();
        }

        //------------------------------------------------------------------
        private void Reset ()
        {
            Color = InitialColor;
            brakes.Visible = false;
        }

        #endregion

        #region Controls

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            Velocity += Acceleration;
//            new Text ("Accelerate", GlobalPosition, Color.DarkOrange);
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Velocity > 0)
                Velocity -= Acceleration * 3;
            brakes.Visible = true;
            //            new Text ("Brake", GlobalPosition, Color.Red);
        }

        //------------------------------------------------------------------

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
            base.Draw (spriteBatch);

            spriteBatch.Draw (Texture, GlobalPosition, null, Color, Angle, origin, 1.0f, SpriteEffects.None, 1.0f);
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
//            new Text (Velocity.ToString (), GlobalPosition, Color.DarkSeaGreen, true);
        }

        //------------------------------------------------------------------
        public override string ToString ()
        {
            return ID.ToString ();
        }

        //------------------------------------------------------------------
        public void EnableBlinker (Lane newLane)
        {
            const int shift = 30;

            if (newLane == Lane.Left)
            {
                blinker.Position = new Vector2 (-shift, 0);
                blinker.Flip (false);
            }
            else if (newLane == Lane.Right)
            {
                blinker.Position = new Vector2 (shift, 0);
                blinker.Flip (true);
            }

            blinker.Enable ();
        }

        //------------------------------------------------------------------
        public void DisableBlinker ()
        {
            blinker.Disable ();
        }
    }
}