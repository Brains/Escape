using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Markers
{
    public class Point : Marker
    {
        public Point (Vector2 position)
        {
            this.Position = position;

            Manager.Markers.Add (this);
        }

        //------------------------------------------------------------------
        public Point (Vector2 position, Color color) : this (position)
        {
            this.Color = color;
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle (Position, 5, 10, Color, 5);
        }
    }
}