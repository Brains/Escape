using System;
using Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Cars.Weights;
using Traffic.Drivers;

namespace Traffic.Cars
{
    public class Car : Object
    {
        //-----------------------------------------------------------------
        private Lane lane;
        private Vector2 origin;
        private Lights brakes;
        public Blinker Blinker { get; private set; }
        private Lights boost;
        private Weight weight;

        //------------------------------------------------------------------
        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;
        protected internal Bounds Bounds;

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
            LocalPosition = new Vector2 (0, horizont);
            InitialColor = Color.White;
            TextureName = "Car";
            Lane = lane;
            
            Velocity = Lane.Velocity;
            Acceleration = 0.2f;
            Deceleration = 1.0f;

            Driver = new Common (this);

            CreateWeight ();
        }

        //------------------------------------------------------------------
        private void CreateWeight ()
        {
            if (Lane.ID >= 0 && Lane.ID < 4)
                weight = new Light (this);
            if (Lane.ID >= 4 && Lane.ID < 8)
                weight = new Medium (this);
            if (Lane.ID >= 8 && Lane.ID < 12)
                weight = new Heavy (this);
            
            TextureName += weight.TextureSuffix;
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            // Load Texture
            Texture = Lane.Road.Images[TextureName];
            origin = new Vector2 (Texture.Width / 2, Texture.Height / 2);
            Lenght = Texture.Height;

            // Driver
            Driver.Setup();

            // Bounding Box
            Bounds = new Bounds (this, Position, origin);
            Bounds.Inflate (-5, -5);
            Add (Bounds);

            // Lights
            brakes = new Lights (this, "Brake");
            brakes.LocalPosition = new Vector2 (0, Texture.Height / 2 + 10);
            Add (brakes);

            boost = new Lights (this, "Acceleration");
            boost.LocalPosition = new Vector2 (0, -Texture.Height / 2 - 10);
            Add (boost);

            Blinker = new Blinker (this, "Blinker");
            Add (Blinker);

            // Setup Components
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
            brakes.Disable();
            boost.Disable();
        }

        #endregion

        #region Controls

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            Velocity += Acceleration;
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Velocity > 0)
                Velocity -= Deceleration;

            brakes.Enable();
        }

        //------------------------------------------------------------------
        public void EnableBlinker (Lane newLane)
        {
            const int shift = 30;

            if (newLane == Lane.Left)
            {
                Blinker.LocalPosition = new Vector2 (-shift, 0);
                Blinker.Flip (false);
            }
            else if (newLane == Lane.Right)
            {
                Blinker.LocalPosition = new Vector2 (shift, 0);
                Blinker.Flip (true);
            }

            Blinker.Enable ();
        }

        //------------------------------------------------------------------
        public void DisableBlinker ()
        {
            Blinker.Disable ();
        }

        //------------------------------------------------------------------
        public void EnableBoost ()
        {
            boost.Enable();
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
            if (lane == null) return;

            var closestCar = Driver.FindClosestCar (lane.Cars);
            if (closestCar == null) return;

            if (!Intersect (closestCar)) return;

            // Destroy all cars
            if (Lives == closestCar.Lives)
            {
                Explose ();
                closestCar.Explose ();
            }
            // Destroy closest Car
            else if (Lives > closestCar.Lives)
            {
                Lives -= closestCar.Lives;
                closestCar.Explose ();
            }
            // Destroy myself
            else if (Lives < closestCar.Lives)
            {
                closestCar.Lives -= Lives;
                Explose ();
            }
        }

        //------------------------------------------------------------------
        public virtual bool Intersect (Car car)
        {
            if (car == this) return false;
            if (!IsIntersectActive()) return false;
            if (!car.IsIntersectActive()) return false;

            return Bounds.Intersects (car.Bounds);
        }

        //------------------------------------------------------------------
        protected void Explose ()
        {
            Destroy ();

            var explosion = new AnimatedTexture (this, Vector2.Zero, 1.5f, 0.0f);
            explosion.Load (Lane.Road.Images["Explosion"], 24, 12);
            explosion.Finish += Delete;
            Add (explosion);
        }

        //------------------------------------------------------------------
        private void Destroy ()
        {
            Velocity = 0;
            Lives = 0;
            InitialColor = Color.Transparent;
            Bounds = null;
            
            Components.Clear ();
        }

        //------------------------------------------------------------------
        public bool IsIntersectActive()
        {
            return Bounds != null;
        }

        #endregion

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            base.Draw (spriteBatch);

            spriteBatch.Draw (Texture, Position, null, Color, Angle, origin, 1.0f, SpriteEffects, 0.5f);
        }

        //------------------------------------------------------------------
        public override string ToString ()
        {
            return ID.ToString ();
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
//            new Text (Velocity.ToString ("F0"), Position, Color.DarkSeaGreen, true);
//            new Text (Lives.ToString (), Position, Color.Red);

            // SafeZone
//            new Line (Position, Position - new Vector2 (0, Driver.SafeZone), Color.IndianRed);
        }
    }
}