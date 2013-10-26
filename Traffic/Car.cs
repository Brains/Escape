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
using Tools;

namespace Traffic
{
    class Car
    {
        private Game game;
        private Texture2D car;
        private Lane lane;
        private Collisions boundingBox;

        public float VerticalPosition { get; set; }
        public float Velocity { get; set; }

        //------------------------------------------------------------------
        public Car (Game game, Lane lane)
        {
            this.game = game;
            this.lane = lane;

            Velocity = lane.Velocity;

        }

        //------------------------------------------------------------------
        private void CreateBoundingBox ()
        {
            var position = new Vector2 (lane.Position, VerticalPosition);
            Vector2 leftBottom = position - new Vector2 (car.Width / 2, car.Height / 2);
            Vector2 size = position + new Vector2 (car.Width / 2, car.Height / 2);

            boundingBox = new Collisions (leftBottom, size);
        }

        //------------------------------------------------------------------
        public void Initialize ()
        {
            
        }

        //------------------------------------------------------------------
        public void LoadContent ( )
        {
//            ContentManager content = (ContentManager) game.Services.GetService (typeof (ContentManager));
            car = game.Content.Load<Texture2D> ("Images/Cars/Car");

            CreateBoundingBox ();
        }

        //------------------------------------------------------------------
        public void UnloadContent ( )
        {

        }

        //------------------------------------------------------------------
        public void Update ()
        {
//            VerticalPosition -= Velocity * 0.1f; 
//            VerticalPosition -= 0.5f;

            new Tools.Markers.Rectangle (boundingBox.Minimum, boundingBox.Maximum);
            new Tools.Markers.Text (boundingBox.Minimum.ToString (), boundingBox.Minimum);
            new Tools.Markers.Text (boundingBox.Maximum.ToString (), boundingBox.Maximum);
        }


        //------------------------------------------------------------------
        public void Draw (SpriteBatch spriteBatch)
        {
            var position = new Vector2 (lane.Position, VerticalPosition);
            var origin = new Vector2 (car.Width / 2, car.Height / 2);

            spriteBatch.Draw (car, position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
