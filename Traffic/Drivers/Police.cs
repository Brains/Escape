using System;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Police : Driver
    {
        //------------------------------------------------------------------
        public Police (Cars.Police police) : base (police)
        {
            Velocity = 450;
            ChangeLaneSpeed = 1.5f;

            AddInLoop (new Shrink (this));
            AddInLoop (new Overtake (this, Car.Lane.Road.Player));
        }

        //------------------------------------------------------------------
        public override void Setup()
        {
            CheckLaneSafeZoneLower = GetSafeZone (0.5f);
            CheckLaneSafeZoneUpper = GetSafeZone (1.0f);
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
//            DrawActions ();

//            // Lives
//            new Text (Car.Lives.ToString (), Car.GlobalPosition, Color.Red);

        }
    }
}