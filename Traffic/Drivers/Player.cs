using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Traffic.Actions;
using Traffic.Cars;
using Tools.Markers;
using Loop = Tools.Timers.Loop;
using Point = Tools.Markers.Point;

namespace Traffic.Drivers
{
    public class Player : Driver
    {
        private bool slaving;

        //------------------------------------------------------------------
        public Player (Car car) : base (car)
        {
            Velocity = 400;
            ChangeLaneSpeed = 2;

            AddInLoop (new Input (this));
//            AddInLoop (new Shrink (this));
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

//            AdjustSpeed ();

//            SetSafeZone();

            CaptureSlave();

            Debug ();
        }

        //------------------------------------------------------------------
        private void CaptureSlave()
        {
            if (slaving) Click();

            var keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Contains (Keys.G))
            {
                slaving = true;

                Settings.TimeScale = 0.1f;
//                AddInSequnce (new Controller ((Action <float>) OnScale, 0.5f, 0.5f));
            }
        }

        private void OnScale (float value)
        {
            Settings.TimeScale -= value;
        }

        //------------------------------------------------------------------
        private bool Click()
        {
            var mouse = Mouse.GetState ();
            var position = new Vector2 (mouse.X, mouse.Y);

            if (mouse.LeftButton != ButtonState.Pressed) return false;
            
            slaving = false;
//            AddInSequnce (new Controller ((Action<float>) OnScale, -0.5f, 0.5f));
            Settings.TimeScale = 1;

            Slave (Car.Lane.Road.FindCar (position));
            return true;
        }

        //------------------------------------------------------------------
        private void Slave (Car car)
        {
            if (car == null) return;

            Car police = car.Lane.Road.FindClosestPolice (car);
            if (police == null) return;

            car.Driver.AddInLoop (new Overtake (car.Driver, police));
            car.Driver.AddInLoop (new Block (car.Driver, police));

            car.Acceleration = car.Acceleration * 2;
            car.Deceleration = car.Deceleration * 2;
            car.Driver.Velocity = car.Driver.Velocity * 2;

            Tools.Timers.Loop.Create (1, 0, car.Turn);
        }

        //------------------------------------------------------------------
        private void SetSafeZone ()
        {
            Car car = Car;
            Car closest = FindClosestCar (Car.Lane.Cars.Where (IsCarAhead));
            if (closest == null) return;

            float approachSpeed = car.Velocity - closest.Velocity;
            float distance = Distance (closest);

            SafeZone.Scale = approachSpeed / 100;// * distance / 100;

            if (SafeZone.Scale > 0.7f)
                SafeZone.Scale = 0.7f;
            else if (SafeZone.Scale < 0.4f)
                SafeZone.Scale = 0.4f;
        }

        //-----------------------------------------------------------------
        private void AdjustSpeed ()
        {
            if (Settings.NoPlayerAdjustSpeed) return;

            float distance = GetMinimumDistance (Car.Lane.Cars.Where (IsCarAhead));

            // A point of the "factor" is to accelerate when (distance > Lenght * 3)
            int factor = Math.Sign (distance / Car.Lenght - 2.5f);

//            float factor = (distance / Car.Lenght - 3) / 9;
//            if (factor > 1) factor = 1.0f;
//            if (factor < -1) factor = -1.0f;
//            Car.Velocity += Car.Acceleration * factor;
//            new Text (factor.ToString (), Vector2.One * 100, Color.DarkViolet);

            if (factor > 0) 
                Accelerate ();
            else
                Brake ();
        }

        //------------------------------------------------------------------
        public void Accelerate ()
        {
            if (Car.Velocity < Velocity) 
            {
                Car.Accelerate ();
                Car.EnableBoost ();
            }
        }

        //------------------------------------------------------------------
        public void Brake ()
        {
//            if (Car.Velocity > 100)
                Car.Brake ();
        }

        //-----------------------------------------------------------------
        private void Debug ()
        {
//            DrawSafeZone();

//            new Text (Car.Lane.ToString(), Car.Position, Color.DarkRed, true);

            // Draw Closest car
//            Car closestCar = FindClosestCar (Car.Lane.Cars.Where (IsCarAhead));
//            if (closestCar != null)
//                closestCar.Color = Color.Red;
        }



    }
}