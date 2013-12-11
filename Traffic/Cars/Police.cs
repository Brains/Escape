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
            Deceleration = 3.0f;

            Driver = new Drivers.Police (this);

            CreateFlasher();
        }

        //------------------------------------------------------------------
        public void CreateFlasher ()
        {
            // Flasher
            flasher = new Lights(this, "Flasher");
            flasher.Enable();
            Add (flasher);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            flasher.Rotation += elapsed * 10;

            base.Update (elapsed);
        }
    }
}