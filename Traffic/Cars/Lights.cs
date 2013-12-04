using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Timers;

namespace Traffic.Cars
{
    public class Lights : Object
    {
        private Texture2D texture;
        private readonly Car car;
        private Vector2 origin;
        private SpriteEffects flip;

        protected Color Color = Color.White;

        public float Rotation { get; set; }

        //------------------------------------------------------------------
        public Lights (Car car, string textureName) : base (car)
        {
            this.car = car;

            LoadTexture (textureName);
        }

        //------------------------------------------------------------------
        private void LoadTexture(string name)
        {
            texture = car.Lane.Road.Images[name];
            origin = new Vector2 (texture.Width / 2, texture.Height / 2);
        }

        //-----------------------------------------------------------------
        public virtual void Turn ( )
        {
            Visible = !Visible;
        }

        //-----------------------------------------------------------------
        public virtual void Enable ()
        {
            Visible = true;
        }

        //-----------------------------------------------------------------
        public virtual void Disable ()
        {
            Visible = false;
        }

        //-----------------------------------------------------------------
        public void Flip (bool set)
        {
            flip = set ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            base.Draw (spriteBatch);

            if (!Visible) return;

            spriteBatch.Draw (texture, Position, null, Color, Rotation, origin, 1.0f, flip, 0.6f);
        }


    }
}