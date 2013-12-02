using System;
using Microsoft.Xna.Framework;
using Tools.Markers;
using Traffic.Actions;

namespace Traffic.Drivers
{
    internal class Police : Driver
    {
        //------------------------------------------------------------------
        public Police (Cars.Car car) : base (car)
        {
            Velocity = 400;
            ChangeLaneSpeed = 2;

            var shrink = new Shrink (this);
            AddInLoop (shrink);
            AddInLoop (new Overtake (this, Car.Lane.Road.Player, shrink));
        }

        //-----------------------------------------------------------------
        public override void Setup()
        {
            CheckLaneSafeZoneLower = GetSafeZone (0.2f);
            CheckLaneSafeZoneUpper = GetSafeZone (1.0f);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            Debug();
        }

        //------------------------------------------------------------------
        private void Debug()
        {
//            DrawActions ();

//            // Lives
//            new Text (Car.Lives.ToString (), Car.Position, Color.Red);
        }
    }
}