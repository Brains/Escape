using Microsoft.Xna.Framework;
using Traffic.Cars.Weights;
using Traffic.Drivers;

namespace Traffic.Cars
{
    public class Police : Car
    {
        private Lights flasher;

        //------------------------------------------------------------------
        public Police(Lane lane, int id, int position) : 
            base(lane, id, position)
        {
            Lives = 20;
            Acceleration = 1.0f;
            Deceleration = 4.0f;

            SetDriver (new Drivers.Police (this));

            CreateFlasher();
        }

        //------------------------------------------------------------------
        public void CreateFlasher ()
        {
            flasher = new Lights(this, "Flasher");

            Add (flasher);
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            CreateDrawable (game, "Police");

            base.Setup (game);

            flasher.Enable ();
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            flasher.Drawable.Rotation += elapsed * 10;

            base.Update (elapsed);
        }
    }
}