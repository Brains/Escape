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
            Visible = !Visible;
        }

        //-----------------------------------------------------------------
        public void Enable ()
        {
            Visible = true;
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

            spriteBatch.Draw (texture, GlobalPosition, null, Color.White, 0, origin, 1.0f, flip, 1.0f);
        }
    }
}