using Microsoft.Xna.Framework;
using Traffic.Drivers;

namespace Traffic.Cars
{
    public class Police : Car
    {
        private Lights flasher;

        //------------------------------------------------------------------
        public Police (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Police (this);
            InitialColor = Color.White;
            TextureName = "Police";

            Lives = 20;

            Acceleration = 1.0f;
            Deceleration = 3.0f;
        }

        //------------------------------------------------------------------
        public override void Setup()
        {
            // Flasher
            flasher = new Lights(this, "Flasher");
            flasher.Enable();
            Add (flasher);

            base.Setup ();
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            flasher.Rotation += elapsed * 10;

            base.Update (elapsed);
        }
    }
}