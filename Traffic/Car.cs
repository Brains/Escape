using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Traffic.Actions;
using Traffic.Drivers;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Tools;
using Tools.Markers;
using Point = Tools.Markers.Point;

namespace Traffic
{
    internal class Car : Object
    {
        //-----------------------------------------------------------------
        public int ID;
        private Vector2 position;
        private Vector2 origin;
        private Rectangle bounds;

        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;
        protected float Acceleration = 5;

        public static float VelocityFactor = 100;
        private float angle;

        public float Velocity { get; set; }
        public Color Color { get; set; }
        public Lane Lane { get; set; }
        public int Lenght { get; set; }
        public Driver Driver { get; set; }
        public int Lives { get; set; }

        //------------------------------------------------------------------
        public Vector2 Position
        {
            get { return position; }
            set
            {
                bounds.X = (int) (value.X - bounds.Width / 2);
                bounds.Y = (int) (value.Y - bounds.Height / 2);
                position = value;
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
            Position = new Vector2 (lane.Position.X, horizont);
            Lives = 1;
        }

        //------------------------------------------------------------------
        public void Create ()
        {
            Texture = Lane.Road.Images[TextureName];
            origin = new Vector2 (Texture.Width / 2, Texture.Height / 2);
            Lenght = Texture.Height;

            Driver.Create ();

            CreateBoundingBox ();
        }

        //------------------------------------------------------------------
        private void CreateBoundingBox ()
        {
            Vector2 leftBottom = Position - origin;

            bounds = new Rectangle ((int) leftBottom.X, (int) leftBottom.Y, Texture.Width, Texture.Height);
            bounds.Inflate (-5, -5);
        }

        #endregion

        #region Update

        //------------------------------------------------------------------
        public virtual void Update ()
        {
            Color = InitialColor;

            Move (-Velocity);

            Driver.Update ();

            DetectCollisions ();

            new Text (ID.ToString (), Position, Color.CadetBlue, true);
        }

        #endregion

        #region Controls

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            if (Velocity < Driver.Velocity)
                Velocity += Acceleration;

//            new Text (Velocity.ToString ("F1"), Position, Color.RoyalBlue, true);
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
            if (Velocity > 0)
                Velocity -= Acceleration;
        }

        //------------------------------------------------------------------
        public void Move (float velocity)
        {
            Position += new Vector2 (0, velocity / VelocityFactor);
        }

        //------------------------------------------------------------------
        void CorrectPositionOnLane ()
        {
            var finalPoint = new Vector2 (Lane.Position.X, Position.Y);
            Position = finalPoint;
        }

        //------------------------------------------------------------------
        public void ChangeLane (Lane lane)
        {
            if (lane == null) return;

            lane.Add (this);
            Lane.Remove (this);

            AnimateGhangingLane (lane);
        }

        //------------------------------------------------------------------
        private void AnimateGhangingLane (Lane lane)
        {
            // ToDo: Below
            // Turn Off ChangeLane while animation
            // When perform Add and Remove from Lane?
            // After animation set precise Position

            var sequence = new Sequence ();

            // Rotate
            Action <float> rotate = share => angle += share;
            float finalAngle = (lane.Position.X < Position.X) ? MathHelper.ToRadians (-5) : MathHelper.ToRadians (5);
            sequence.Add (new Controller (rotate, finalAngle, 0.1f));

            // Moving
            Action <Vector2> move = shift => Position += shift;
            var diapason = new Vector2 (lane.Position.X - Position.X, 0);
            sequence.Add (new Controller (move, diapason, 0.2f));

            // Inverse rotating
            var inverseRotating = new Controller (rotate, -finalAngle, 0.1f);
            sequence.Add (inverseRotating);

            // Level float rounding error
            sequence.Add (new Generic (CorrectPositionOnLane));

            sequence.AddToManager ();
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

//            foreach (var car in lane.Cars.Where (Intersect))
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
            spriteBatch.Draw (Texture, Position, null, Color, angle, origin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}