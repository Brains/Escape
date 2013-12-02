using Microsoft.Xna.Framework;

namespace Traffic.Cars
{
    public class Bounds : Object
    {
        private Rectangle rectangle;

        //------------------------------------------------------------------
        private Rectangle Rectangle
        {
            get
            {
                rectangle.X = (int) (Position.X - rectangle.Width / 2);
                rectangle.Y = (int) (Position.Y - rectangle.Height / 2);

                return rectangle;
            }
        }

        //------------------------------------------------------------------
        public Bounds (Object root, Vector2 position, Vector2 origin) : base (root)
        {
            Vector2 leftBottom = position - origin;
            rectangle = new Rectangle ((int) leftBottom.X, (int) leftBottom.Y, (int) (origin.X * 2), (int) (origin.Y * 2));
        }


        //------------------------------------------------------------------
        public void Inflate (int x, int y)
        {
            rectangle.Inflate (x, y);
        }

        //------------------------------------------------------------------
        public bool Intersects (Bounds bounds)
        {
            var @from = new Vector2 (Rectangle.X, Rectangle.Y);
            var to = new Vector2 (Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height);
//            new Tools.Markers.Rectangle (@from, to);

            return Rectangle.Intersects (bounds.Rectangle);
        }
    }
}