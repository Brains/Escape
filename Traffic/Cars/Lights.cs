using System.Reflection;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic.Cars
{
    public class Lights : Object
    {
        private readonly Car car;
        private readonly string textureName;

        //------------------------------------------------------------------
        public Lights (Car car, string textureName) : base (car)
        {
            this.textureName = textureName;
            this.car = car;
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            base.Setup (game);

            CreateDrawable (game, textureName);
            Drawable.Depth = 0.9f;
        }

        //-----------------------------------------------------------------
        public virtual void Turn ( )
        {
            Drawable.Visible = !Drawable.Visible;
        }

        //-----------------------------------------------------------------
        public virtual void Enable ()
        {
            Drawable.Visible = true;
        }

        //-----------------------------------------------------------------
        public virtual void Disable ()
        {
            Drawable.Visible = false;
        }

        //-----------------------------------------------------------------
        public void Flip (bool set)
        {
            Drawable.Flip = set ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }
    }
}