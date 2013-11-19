using System;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Police : Driver
    {
        public override float DangerousZone
        {
            // Hardcoded numbers are constants
            get { return base.DangerousZone / 1; }
        }

        //------------------------------------------------------------------
        public Police (Cars.Police police) : base (police)
        {
            Velocity = 500;

            AddInLoop (new Shrink (this));
            AddInLoop (new Overtake (this, Car.Lane.Road.Player));
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            Debug ();
        }

        //------------------------------------------------------------------
        private void Debug ()
        {
            // Draw DangerousZone
            var pos = Car.GlobalPosition;
            new Line (pos, pos - new Vector2 (0, DangerousZone), Color.IndianRed);
        }
    }
}