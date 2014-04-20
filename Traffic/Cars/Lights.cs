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
        private string textureName;

        public float Rotation { get; set; }

        //------------------------------------------------------------------
        public Lights (Car car, string textureName) : base (car)
        {
            this.textureName = textureName;
            this.car = car;

            LoadTexture (textureName);
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            base.Setup (game);

//            CreateDrawable (game, textureName);

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
//            Drawable.Visible = !Drawable.Visible;
        }

        //-----------------------------------------------------------------
        public virtual void Enable ()
        {
//            Drawable.Visible = true;
        }

        //-----------------------------------------------------------------
        public virtual void Disable ()
        {
//            Drawable.Visible = false;
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

//            if (!Drawable.Visible) return;

//            spriteBatch.Draw (texture, Position, null, Color, Rotation, origin, 1.0f, flip, 0.6f);
        }


    }
}