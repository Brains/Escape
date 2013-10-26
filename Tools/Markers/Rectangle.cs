using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Markers
{
    public class Rectangle : Marker
    {
        public Vector2 From { get; set; }
        public Vector2 To { get; set; }

        //------------------------------------------------------------------
        public Rectangle (Vector2 @from, Vector2 to)
        {
            From = @from;
            To = to;

            Manager.Markers.Add (this);
        }

        //------------------------------------------------------------------
        public Rectangle (Vector2 @from, Vector2 to, Color color) : this (@from, to)
        {
            Color = color;
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            Vector2 size = To - From;

            spriteBatch.DrawRectangle (From, size, Color);
        }
    }
}