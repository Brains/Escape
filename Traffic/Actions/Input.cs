using System.Linq;
using Microsoft.Xna.Framework.Input;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;

namespace Traffic.Actions
{
    internal class Input : Sequence
    {
        private Drivers.Player player;
        private static KeyboardState current;
        private static KeyboardState previous;

        //------------------------------------------------------------------
        public Input (Drivers.Player player)
        {
            this.player = player;
            Name = "Input";
            Add (new Generic (CheckInput) {Name = "CheckInput"});
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
            int factor = 5;

            if (IsKeyPressed (Keys.Right)) ChangeLane (player.Car.Lane.Right);
            if (IsKeyPressed (Keys.Left)) ChangeLane (player.Car.Lane.Left);
            
            if (IsKeyDown (Keys.Down)) 
                foreach (var index in Enumerable.Range (0, factor))
                    player.Brake ();
            
            if (IsKeyDown (Keys.Up))
                foreach (var index in Enumerable.Range (0, factor))
                    player.Accelerate ();
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
            player.Car.Velocity += player.Car.Acceleration;
        }

        //------------------------------------------------------------------
        private void ChangeLane (Lane lane)
        {
            // Add blinker as Parallel action sequence
//            Sequence blinker = new Sequence();
//            player.EnableBlinker (lane, blinker, 1.0f);
//            player.AddInLoop (blinker);

            player.ChangeLane (lane, this, player.GetChangeLanesDuration () / 2.0f);
                // Divide by two because blinkers are turned off and take no time
        }
    }
}