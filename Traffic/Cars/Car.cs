using System;
using Fluid;
using Engine;
using Microsoft.Xna.Framework;
using Traffic.Cars.Weights;
using Traffic.Drivers;

namespace Traffic.Cars
{
    public class Car : Engine.Object
    {
        // Components
        protected internal Bounds Bounds;
        
        // Lights
        private Lights brakes;
        private Blinker blinker;
        private Lights boost;

        // Properties
        public readonly int ID;
        public Lane Lane { get; private set; }
        public Driver Driver { get; private set; }
        public float Velocity { get; set; }
        public int Lenght { get; set; }
        public int Lives { get; set; }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }

        #region Creation

        //------------------------------------------------------------------
        public Car (Lane lane, int id, int position) : base(lane)
        {
            LocalPosition = new Vector2 (0, position);
            SetLane (lane);
            ID = id;

            Velocity = Lane.Velocity;
            Lives = GeLives();
            Acceleration = 1.0f;// * weight.Acceleration;
            Deceleration = 1.5f;// * weight.Deceleration;

            SetDriver (new Common (this));
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            // If Drawable wasn't created into derived classes
            if (Drawable == null)
                CreateDrawable (game, "Car " + GetTextureNameSuffix());
            
            Lenght = Drawable.Height;
            CreateBoundingBox ();
            CreateLights ();

            base.Setup (game);
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
            Bounds = new Bounds (this, Position, Drawable.Origin);
            Bounds.Inflate (-5, -5);

            Add (Bounds);
        }

        //------------------------------------------------------------------
        private void CreateLights ()
        {
            brakes = new Lights (this, "Brake");
            brakes.LocalPosition = new Vector2 (0, Lenght / 2 + 10);
            Add (brakes);

            boost = new Lights (this, "Acceleration");
            boost.LocalPosition = new Vector2 (0, -Lenght / 2 - 10);
            Add (boost);

            blinker = new Blinker (this, "Blinker");
            Add (blinker);
        }



        #endregion

        #region Update

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);
            
            Reset ();

            MoveByVelocity(elapsed);
            SimulateCameraMoving(elapsed);

            DetectCollisions ();

            Debug ();
        }

        //------------------------------------------------------------------
        private void MoveByVelocity (float elapsed)
        {
            float shift = -Velocity * elapsed;

            Move (new Vector2 (0, shift));
        }

        //------------------------------------------------------------------
        private void SimulateCameraMoving (float elapsed)
        {
            float shift = Lane.Road.Player.Velocity * elapsed;

            Move (new Vector2 (0, shift));
        }

        //------------------------------------------------------------------
        private void Reset ()
        {
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
            return blinker.Drawable.Visible;
        }

        //------------------------------------------------------------------
        public void EnableBoost ()
        {
            boost.Enable();
        }

        //------------------------------------------------------------------
        public void Turn ()
        {
            Drawable.Color = Drawable.Color == Color.White ? Color.Orange : Color.White;
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
                Explose (closestCar);
                closestCar.Explose (this);
            }
            // Destroy closest Car
            else if (Lives > closestCar.Lives)
            {
                Lives -= closestCar.Lives;
                closestCar.Explose (this);
            }
            // Destroy myself
            else if (Lives < closestCar.Lives)
            {
                closestCar.Lives -= Lives;
                Explose (closestCar);
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
        protected void Explose (Car killer)
        {
            // Drawable.Explose ();

            Destroy ();
        }

        //------------------------------------------------------------------
        private void Destroy ()
        {
            Velocity = 0;
            Lives = 0;
            Bounds = null;
            
            Components.Clear ();

            Delete ();
        }

        //------------------------------------------------------------------
        public bool IsIntersectActive()
        {
            return Bounds != null;
        }

        #endregion


        #region Lanes

        //------------------------------------------------------------------
        public void DockToLane()
        {
            LocalPosition = new Vector2 (0, Position.Y);
        }

        //-----------------------------------------------------------------
        public void SetLane (Lane lane)
        {
            Lane = lane;
            Root = lane;
        }

        #endregion

        //-----------------------------------------------------------------
        protected void SetDriver (Driver driver)
        {
            Remove (driver);
            Driver = driver;
            Add (driver);
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