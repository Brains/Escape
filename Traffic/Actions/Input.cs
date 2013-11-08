using Microsoft.Xna.Framework.Input;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Input : Loop
    {
        private Driver driver;
        private static KeyboardState current;
        private static KeyboardState previous;

        //------------------------------------------------------------------
        public Input (Driver driver)
        {
            this.driver = driver;
            Name = "Input";
            Initial = new Generic (CheckInput) {Name = "CheckInput"};
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            previous = current;
            current = Keyboard.GetState ();
        }

        //------------------------------------------------------------------
        public void CheckInput ()
        {
            if (IsKeyPressed (Keys.Right)) ChangeLane (driver.Car.Lane.Right);
            if (IsKeyPressed (Keys.Left)) ChangeLane (driver.Car.Lane.Left);
            if (IsKeyDown (Keys.Down)) driver.Car.Brake ();
            if (IsKeyDown (Keys.Up)) ForceAccelerate ();
        }

        //------------------------------------------------------------------
        public static bool IsKeyPressed (Keys key)
        {
            return (current.IsKeyDown (key) && previous.IsKeyUp (key));
        }

        //------------------------------------------------------------------
        public static bool IsKeyDown (Keys key)
        {
            return current.IsKeyDown (key);
        }

        //------------------------------------------------------------------
        protected void ForceAccelerate ()
        {
            driver.Car.Velocity += driver.Car.Acceleration;
        }

        //------------------------------------------------------------------
        private void Accelerate ()
        {
            if (driver.Car.Velocity < 300)
                driver.Car.Accelerate ();
        }

        //------------------------------------------------------------------
        private void Brake ()
        {
            if (driver.Car.Velocity > 150)
                driver.Car.Brake ();
        }

        //------------------------------------------------------------------
        private void ChangeLane (Lane lane)
        {
            // Add blinker as Parallel action sequence
            Sequence blinker = new Sequence();
            driver.EnableBlinker (lane, blinker, 1.0f);
            driver.AddParallel (blinker);

            driver.ChangeLane (lane, this);
        }
    }
}