using Microsoft.Xna.Framework.Input;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Input : Loop
    {
        private Drivers.Player driver;
        private static KeyboardState current;
        private static KeyboardState previous;

        //------------------------------------------------------------------
        public Input (Drivers.Player driver)
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
            if (IsKeyDown (Keys.Down)) driver.Brake ();
            if (IsKeyDown (Keys.Up)) driver.Accelerate ();
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
        private void ChangeLane (Lane lane)
        {
            // Add blinker as Parallel action sequence
//            Sequence blinker = new Sequence();
//            driver.EnableBlinker (lane, blinker, 1.0f);
//            driver.AddParallel (blinker);

            driver.ChangeLane (lane, this, driver.GetChangeLanesDuration () / 2.0f); // Divide by two because blinkers are turned off and take no time
        }
    }
}