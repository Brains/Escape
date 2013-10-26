using Microsoft.Xna.Framework;

namespace Physics
{
    public class Collisions
    {
        #region Enumerations

        public enum Type
        {
            Inside,
            Outside,
            Intersects
        }

        #endregion Enumerations

        //------------------------------------------------------------------
        #region Fields

        public Vector2 Maximum { get; set; }
        public Vector2 Minimum { get; set; }

        #endregion Fields

        //------------------------------------------------------------------
        #region Constructors

        public Collisions ()
        {
        }

        //------------------------------------------------------------------
        public Collisions (Vector2 min, Vector2 max)
        {
            Minimum = (min);
            Maximum = (max);
        }

        //------------------------------------------------------------------
        public Collisions (float x, float y, float w, float h)
        {
            Minimum = new Vector2 (x, y);
            Maximum = new Vector2 (x + w, y + h);
        }

        #endregion Constructors

        //------------------------------------------------------------------
        #region Methods

        //------------------------------------------------------------------
        public void Move (Vector2 shift)
        {
            Minimum += shift;
            Maximum += shift;
        }

        //------------------------------------------------------------------
        public Vector2 GetSize ()
        {
            return Maximum - Minimum;
        }

        //------------------------------------------------------------------
        public Type Intersect (Collisions box)
        {
            if (box.Minimum.X >= Minimum.X && box.Maximum.X <= Maximum.X && box.Minimum.Y >= Minimum.Y && box.Maximum.Y <= Maximum.Y)
                return Type.Inside;
            if (Maximum.X < box.Minimum.X || Minimum.X > box.Maximum.X)
                return Type.Outside;
            if (Maximum.Y < box.Minimum.Y || Minimum.Y > box.Maximum.Y)
                return Type.Outside;

            return Type.Intersects;
        }

        //------------------------------------------------------------------
        public Collisions Scale (float kof)
        {
            float w = Maximum.X - Minimum.X;
            float h = Maximum.Y - Minimum.Y;

            float newW = w*kof;
            float newH = h*kof;

            float newX = Minimum.X + (w - newW)/2;
            float newY = Minimum.Y + (h - newH)/2;

            return new Collisions (newX, newY, newW, newH);
        }

        //------------------------------------------------------------------
        public Collisions Scale (Vector2 kof)
        {
            float w = Maximum.X - Minimum.X;
            float h = Maximum.Y - Minimum.Y;
            
            float newW = w*kof.X;
            float newH = h*kof.Y;
            float newX = Minimum.X + (w - newW)/2;
            float newY = Minimum.Y + (h - newH)/2;
            
            return new Collisions (newX, newY, newW, newH);
        }
        #endregion Methods
    }
}