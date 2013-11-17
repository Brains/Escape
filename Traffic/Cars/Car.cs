using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Cars.Weights;
using Traffic.Drivers;

namespace Traffic.Cars
{
    internal class Car : Object
    {
        //-----------------------------------------------------------------
        private Lane lane;
        private Vector2 origin;

        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;
        protected internal Bounds Bounds;
        private Lights brakes;
        private Lights blinker;
        private Lights boost;
        private Weights.Weight weight;

        //------------------------------------------------------------------
        public int ID;
        public float Velocity { get; set; }
        public Color Color { get; set; }
        public int Lenght { get; set; }
        public Driver Driver { get; set; }
        public int Lives { get; set; }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }
        public float Angle { get; set; }
        public SpriteEffects SpriteEffects { get; set; }

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
            Lives = 3;
            Acceleration = 0.2f;
            Deceleration = 1.0f;
            Velocity = Lane.Velocity;
            CreateWeight ();
            TextureName += weight.TextureSuffix;
        }

        //------------------------------------------------------------------
        private void CreateWeight ()
        {
            if (Lane.Properties.ID >= 0 && Lane.Properties.ID < 4)
                weight = new Light (this);
            if (Lane.Properties.ID >= 4 && Lane.Properties.ID < 8)
                weight = new Medium (this);
            if (Lane.Properties.ID >= 8 && Lane.Properties.ID < 12)
                weight = new Heavy (this);
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            // Load Texture
            Texture = Lane.Road.Images[TextureName];
            origin = new Vector2 (Texture.Width / 2, Texture.Height / 2);
            Lenght = Texture.Height;

            // Bounding Box
            Bounds = new Bounds (this, GlobalPosition, origin);
            Bounds.Inflate (-5, -5);
            Add (Bounds);

            // Lights
            brakes = new Lights (this, "Brake");
            brakes.Position = new Vector2 (0, Texture.Height / 2 + 10);
            Add (brakes);

            boost = new Lights (this, "Acceleration");
            boost.Position = new Vector2 (0, -Texture.Height / 2 - 10);
            Add (boost);

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

            Move (-Velocity * elapsed);

            // Simulate Camera moving
            Move (Lane.Road.Player.Velocity * elapsed);

            Driver.Update (elapsed);

            DetectCollisions ();

            Debug ();
        }

        //------------------------------------------------------------------
        private void Reset ()
        {
            Color = InitialColor;
            brakes.Visible = false;
            boost.Visible = false;
        }

        #endregion

        #region Controls

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            Velocity += Acceleration;
            
            boost.Visible = true;
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Velocity > 0)
                Velocity -= Deceleration;

            brakes.Visible = true;
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

                Lives --;//= closestCar.Lives;
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
        public virtual bool Intersect (Car car)
        {
            if (car == this) return false;

            return Bounds.Intersects (car.Bounds);
        }

        #endregion

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            base.Draw (spriteBatch);

            spriteBatch.Draw (Texture, GlobalPosition, null, Color, Angle, origin, 1.0f, SpriteEffects, 1.0f);
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

        //------------------------------------------------------------------
        public override string ToString ()
        {
            return ID.ToString ();
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
//            new Text (Velocity.ToString ("F0"), GlobalPosition, Color.DarkSeaGreen, true);
//            new Line (GlobalPosition, GlobalPosition - new Vector2 (0, Driver.DangerousZone / 1.5f), Color.IndianRed);
        }
    }
}