using System;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Physics;
using Tools;
using Tools.Markers;
using Point = Tools.Markers.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Traffic
{
    internal class Car
    {
        //------------------------------------------------------------------
        private Vector2 position;
        private Vector2 origin;
        private Rectangle bounds;
        public static int VelocityFactor = 100;

        protected Texture2D Texture;
        protected Color InitialColor;
        protected string TextureName;

        //------------------------------------------------------------------
        // ToDo: remove Car.Position (use only Move)
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
        public Car (Lane lane, int getInsertPosition)
        {
            Lane = lane;
            InitialColor = Color.NavajoWhite;
            TextureName = "Car";
            Position = new Vector2 (lane.Position.X, getInsertPosition);
            Velocity = lane.Velocity;
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

            Move (-Velocity);

            new Text (Velocity.ToString ("F0"), Position, Color.Maroon, true);
            
        }

        //------------------------------------------------------------------
        public void Move (float shift)
        {
            Position += new Vector2 (0, shift / VelocityFactor);
        }

        //------------------------------------------------------------------
        public bool Intersect (Car car)
        {
            var @from = new Vector2 (bounds.X, bounds.Y);
            var to = new Vector2 (bounds.X + bounds.Width, bounds.Y + bounds.Height);
            new Tools.Markers.Rectangle (@from, to);


            if (car == this) return false;

            return bounds.Intersects (car.bounds);
        }


        //------------------------------------------------------------------
        public bool ChangeOnLeftLane ()
        {
            if (Lane.Left == null) return false;
            if (!Lane.Left.IsFreeSpace (Position.Y, Height))
            {
//                Tools.Markers.Manager.Clear = false;
//                new Text ("No Space", Position, Color.Red);
                return false;
            }

            Lane.Left.Add (this);
            Lane.Remove (this);

            return true;
        }

        //------------------------------------------------------------------
        public bool ChangeOnRightLane ()
        {
            if (Lane.Right == null) return false;
            if (!Lane.Right.IsFreeSpace (Position.Y, Height))
            {
//                Tools.Markers.Manager.Clear = false;
//                new Text ("No Space", Position, Color.Red);
                return false;
            }
            Lane.Right.Add (this);
            Lane.Remove (this);

            return true;
        }

        #endregion

        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (Texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}