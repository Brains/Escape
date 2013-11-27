using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic.Cars
{
    class Lights : Object
    {
        private Texture2D texture;
        private readonly Car car;
        private readonly string textureName;
        private Vector2 origin;
        private SpriteEffects flip;
        private Color color = Color.White;

        //------------------------------------------------------------------
        public bool Blink { get; set; }

        //------------------------------------------------------------------
        public Lights (Car car, string textureName) : base (car)
        {
            this.car = car;
            this.textureName = textureName;
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            base.Setup ();

            texture = car.Lane.Road.Images[textureName];
            origin = new Vector2 (texture.Width / 2, texture.Height / 2);
        }

        //-----------------------------------------------------------------
        public void Turn ( )
        {
//            Visible = !Visible;
            color = color == Color.White ? Color.Transparent : Color.White;
        }

        //-----------------------------------------------------------------
        public void Enable ()
        {
            Visible = true;

            if (Blink)
                Tools.Timers.Loop.Create (0.2f, 10, Turn);
        }

        //-----------------------------------------------------------------
        public void Disable ()
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

            spriteBatch.Draw (texture, GlobalPosition, null, color, 0, origin, 1.0f, flip, 0.5f);
        }
    }
}