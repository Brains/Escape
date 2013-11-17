using Microsoft.Xna.Framework;

namespace Traffic.Cars
{
    internal class Player : Car
    {
        private Car lastCollided;

        //------------------------------------------------------------------
        public Player (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Player (this);
            InitialColor = Color.White;
            TextureName = "Player";
            Lives = 99;
            Acceleration = 1.5f;
            Deceleration = 3.0f;
        }

        //------------------------------------------------------------------
        public override bool Intersect (Car car)
        {
            if (car == this) return false;
            if (car == lastCollided) return false;

            if (Bounds.Intersects (car.Bounds))
            {
                lastCollided = car;
                return true;
            }


            return false;
        }
    }
}