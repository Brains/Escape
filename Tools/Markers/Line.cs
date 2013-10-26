using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Markers
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

            Manager.Markers.Add (this);
        }

        //------------------------------------------------------------------
        public Line (Vector2 from, Vector2 to, Color color) : this (from, to)
        {
            Color = color;
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine (From, To, Color);
        }
    }
}