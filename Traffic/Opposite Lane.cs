using Microsoft.Xna.Framework.Graphics;
using Traffic.Cars;

namespace Traffic
{
    internal class OppositeLane : Lane
    {
        //------------------------------------------------------------------
        public OppositeLane (Road road, int id) : base (road, id)
        {
            IsOpposite = true;
        }

        //------------------------------------------------------------------
        protected override Car CreateCar ()
        {
            var car = base.CreateCar ();
            car.SpriteEffects = SpriteEffects.FlipVertically;

            return car;
        }
    }
}