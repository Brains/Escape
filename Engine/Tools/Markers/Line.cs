using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Tools.Markers
{
    public class Line : Marker
    {
        public Vector2 From { get; set; }
        public Vector2 To { get; set; }

        //------------------------------------------------------------------
        public Line (Vector2 from, Vector2 to)
        {
            From = @from;
            To = to;
        }

        //------------------------------------------------------------------
        public Line (Vector2 from, Vector2 to, Color color) : this (from, to)
        {
            Color = color;
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            Primitives.DrawLine (spriteBatch, From, To, Color);
            Primitives.DrawCircle (spriteBatch, To, 2, 10, Color, 5);
        }
    }
}