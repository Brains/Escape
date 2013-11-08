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
            Initial = new Generic (CheckInput) { Name = "CheckInput" };
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
            if (IsKeyPressed (Keys.Right)) driver.ChangeLane (driver.Car.Lane.Right, this);
            if (IsKeyPressed (Keys.Left)) driver.ChangeLane (driver.Car.Lane.Left, this);
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
    }
}