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
        private Driver driver;
        private Vector2 origin;
        private Lights brakes;
        private Blinker blinker;
        private Lights boost;

        //------------------------------------------------------------------
        protected Texture2D Texture;
        protected Color InitialColor;
        protected internal Bounds Bounds;

        //------------------------------------------------------------------
        public readonly int ID;
        public float Velocity { get; set; }
        public Color Color { get; set; }
        public int Lenght { get; set; }
        public int Lives { get; set; }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }
        public float Rotation { get; set; }
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

        //------------------------------------------------------------------
        public Driver Driver
        {
            get { return driver; }
            set
            {
                Remove (driver);
                driver = value;
                Add (driver);
            }
        }

        #region Creation

        //------------------------------------------------------------------
        public Car (Lane lane, int id, int position) : base(lane)
        {
            LocalPosition = new Vector2 (0, position);
            InitialColor = Color.White;
            Lane = lane;
            ID = id;
            
            Velocity = Lane.Velocity;
            Lives = GeLives();
            Acceleration = 0.2f;
            Deceleration = 1.0f;

            LoadTexture ("Car");
            CreateBoundingBox ();
            CreateLights ();

            Driver = new Common (this);
        }

        //------------------------------------------------------------------
        private int GeLives()
        {
            var weight = Lane.GetWeight();
            
            if (weight == Lane.Weight.Light) 
                return 1;
            if (weight == Lane.Weight.Medium) 
                return 2;
            if (weight == Lane.Weight.Heavy) 
                return 3;

            throw new NotSupportedException ();
        }

        //------------------------------------------------------------------
        private void LoadTexture(string name)
        {
            Texture = Lane.Road.Images[name + GetTextureNameSuffix()];
            origin = new Vector2 (Texture.Width / 2.0f, Texture.Height / 2.0f);
            Lenght = Texture.Height;
        }

        private string GetTextureNameSuffix()
        {
            var weight = Lane.GetWeight ();

            if (weight == Lane.Weight.Light)
                return "(Light)";
            if (weight == Lane.Weight.Medium)
                return "(Medium)";
            if (weight == Lane.Weight.Heavy)
                return "(Heavy)";

            throw new NotSupportedException ();
        }

        //------------------------------------------------------------------
        private void CreateBoundingBox ()
        {
            Bounds = new Bounds (this, Position, origin);
            Bounds.Inflate (-5, -5);
            Add (Bounds);
        }

        //------------------------------------------------------------------
        private void CreateLights ()
        {
            brakes = new Lights (this, "Brake");
            brakes.LocalPosition = new Vector2 (0, Texture.Height / 2 + 10);
            Add (brakes);

            boost = new Lights (this, "Acceleration");
            boost.LocalPosition = new Vector2 (0, -Texture.Height / 2 - 10);
            Add (boost);

            blinker = new Blinker (this, "Blinker");
            Add (blinker);
        }



        #endregion

        #region Update

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            Reset ();
            
            base.Update (elapsed);

            Move (-Velocity * elapsed);

            // Simulate Camera moving
            Move (Lane.Road.Player.Velocity * elapsed);

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
                blinker.LocalPosition = new Vector2 (-shift, 0);
                blinker.Flip (false);
            }
            else if (newLane == Lane.Right)
            {
                blinker.LocalPosition = new Vector2 (shift, 0);
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
        public bool	 IsBlinkerEnable ()
        {
            return blinker.Visible;
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

            spriteBatch.Draw (Texture, Position, null, Color, Rotation, origin, 1.0f, SpriteEffects, 0.5f);
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

        public void DockToLane()
        {
            LocalPosition = new Vector2 (0, Position.Y);
        }
    }
}