using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Traffic.Drivers;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Tools;
using Tools.Markers;
using Point = Tools.Markers.Point;

namespace Traffic
{
    internal class Car
    {
        //-----------------------------------------------------------------
        private Driver driver;
        private Vector2 position;
        private Vector2 origin;
        private Rectangle bounds;

        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;

        protected float MaximumVelocity;
        protected float MinimumVelocity = 0;
        protected float Acceleration = 0.01f;

        public static float VelocityFactor = 100;

        //------------------------------------------------------------------
        public Vector2 Position
        {
            get { return position; }
            set
            {
                bounds.X = (int) (value.X - origin.X);
                bounds.Y = (int) (value.Y - origin.Y);
                position = value;
            }
        }

        public float Velocity { get; set; }
        public Color Color { get; set; }
        public Lane Lane { get; set; }
        public int Height { get; set; }

        #region Creation

        //------------------------------------------------------------------
        public Car (Lane lane, int horizont)
        {
            driver = new Driver (this);
            Lane = lane;
            InitialColor = Color.NavajoWhite;
            TextureName = "Car";
            Position = new Vector2 (lane.Position.X, horizont);
            
            CalculateMaximumVelocity ();
        }

        //------------------------------------------------------------------
        public void Create ()
        {
            Texture = Lane.Road.Images[TextureName];
            origin = new Vector2 (Texture.Width / 2, Texture.Height / 2);
            Height = Texture.Height;

            CreateBoundingBox ();
        }

        //------------------------------------------------------------------
        public virtual void CalculateMaximumVelocity ()
        {
            MaximumVelocity = Lane.Velocity - Lane.Random.Next ((int) (Lane.Velocity * 0.4));
            Velocity = MaximumVelocity;
        }

        //------------------------------------------------------------------
        private void CreateBoundingBox ()
        {
            Vector2 leftBottom = Position - origin;

            bounds = new Rectangle ((int) leftBottom.X, (int) leftBottom.Y, Texture.Width, Texture.Height);
        }

        #endregion

        #region Update

        //------------------------------------------------------------------
        public virtual void Update ()
        {
            Color = InitialColor;

            Accelerate ();

            Move (-Velocity);

            driver.Update ();

            DetectCollisions ();
        }

        #endregion

        #region Controls

        //------------------------------------------------------------------
        protected void Accelerate ()
        {
            if (Velocity < MaximumVelocity)
                Velocity += (Velocity * Acceleration);

            new Text (Velocity.ToString ("F1"), Position, Color.RoyalBlue, true);
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Velocity > MinimumVelocity)
                Velocity -= (Velocity * Acceleration * 3);
        }

        //------------------------------------------------------------------
        public void Move (float shift)
        {
            Position += new Vector2 (0, shift / VelocityFactor);
        }

        //------------------------------------------------------------------
        public void Move (Vector2 shift)
        {
            Position += shift / VelocityFactor;
        }

        //------------------------------------------------------------------
        public bool CheckLane (Lane lane)
        {
            if (lane == null) return false;

            // Debug
            if (!lane.IsFreeSpace (Position.Y, Height))
            {
//                new Tools.Markers.Text ("Danger", new Vector2 (lane.Position.X, Position.Y), Color.IndianRed);
            }

            return lane.IsFreeSpace (Position.Y, Height);
        }

        //------------------------------------------------------------------
        public void ChangeLane (Lane lane)
        {
            if (lane == null) return;

            lane.Add (this);
            Lane.Remove (this);
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

            // ToDo: Use GetClosestCar

            foreach (var car in lane.Cars)
            {
                if (Intersect (car))
                    lane.Remove (car);
            }
        }

        //------------------------------------------------------------------
        public bool Intersect (Car car)
        {
            var @from = new Vector2 (bounds.X, bounds.Y);
            var to = new Vector2 (bounds.X + bounds.Width, bounds.Y + bounds.Height);
//            new Tools.Markers.Rectangle (@from, to);

            if (car == this) return false;

            return bounds.Intersects (car.bounds);
        }

        #endregion

        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (Texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}