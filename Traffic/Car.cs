using System;
using System.Collections.Generic;
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

namespace Traffic
{
    class Car 
    {
        private Game game;
        protected Texture2D texture;
        public Lane Lane { get; set; }
        private Rectangle bounds;
        private Vector2 position;
        private Vector2 origin;

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

        //------------------------------------------------------------------
        public Car (Game game, Lane lane)
        {
            this.game = game;
            this.Lane = lane;
            this.Color = Color.White;

            Velocity = lane.Velocity;

            bounds = new Rectangle ();
        }

        //------------------------------------------------------------------
        private void CreateBoundingBox ()
        {
            Vector2 leftBottom = Position - new Vector2 (texture.Width / 2, texture.Height / 2);

            bounds = new Rectangle ((int) leftBottom.X, (int) leftBottom.Y, texture.Width, texture.Height);
        }

        //------------------------------------------------------------------
        public virtual void Initialize ()
        {

        }

        //------------------------------------------------------------------
        public virtual void LoadContent ( )
        {
            texture = game.Content.Load <Texture2D> ("Images/Cars/Car");
            
            // ToDo: It working only if Player has the same sizes as a Car
            origin = new Vector2 (texture.Width / 2, texture.Height / 2);
            CreateBoundingBox ();
        }

        //------------------------------------------------------------------
        public void UnloadContent ( )
        {

        }

        //------------------------------------------------------------------
        public virtual void Update ()
        {
            // Collisions
            var @from = new Vector2 (bounds.X, bounds.Y);
            var to = new Vector2 (bounds.X + bounds.Width, bounds.Y + bounds.Height);
            new Tools.Markers.Rectangle (@from, to);
            
            new Tools.Markers.Text (Lane.ToString (), Position + new Vector2 (15, 0));
        }

        //------------------------------------------------------------------
        public bool Intersect (Car car)
        {
            if (car == this) return false;
                
            return bounds.Intersects (car.bounds);
        }
        

        //------------------------------------------------------------------
        public void ChangeOnLeftLane ()
        {
            if (Lane.Left != null)
            {
                Lane.Left.Add (this);
                Lane.Remove (this);
            }
        }

        //------------------------------------------------------------------
        public void ChangeOnRightLane ()
        {
            if (Lane.Right != null)
            {
                Lane.Right.Add (this);
                Lane.Remove (this);
            }
        }

        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
