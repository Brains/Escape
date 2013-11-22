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
            get { return base.DangerousZone ; }
        }

        //------------------------------------------------------------------
        public Police (Cars.Police police) : base (police)
        {
            Velocity = 400;

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

        }
    }
}