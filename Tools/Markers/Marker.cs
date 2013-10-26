using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Markers
{
    public abstract class Marker
    {
        //------------------------------------------------------------------
        protected Vector2 Position;
        protected Color Color = Color.DarkGray; // Default color

        //------------------------------------------------------------------
        public abstract void Draw (SpriteBatch spriteBatch);
    }
}