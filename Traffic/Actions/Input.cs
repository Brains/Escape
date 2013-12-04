using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Traffic.Actions.Base;
using Traffic.Cars;
using Traffic.Drivers;
using Action = Traffic.Actions.Base.Action;

namespace Traffic.Actions
{
    internal class Input : SequenceInitial
    {
        private Drivers.Player player;
        private static KeyboardState current;
        private static KeyboardState previous;

        //------------------------------------------------------------------
        public Input (Drivers.Player player)
        {
            this.player = player;
            Initial = new Generic (CheckInput);
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
            UpdateTouch ();
            UpdateKeyboard ();
        }

        //------------------------------------------------------------------
        private void UpdateKeyboard ()
        {
            const int factor = 5;

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
        public void UpdateTouch ()
        {
            //Get the state of the touch panel
            TouchCollection touches = TouchPanel.GetState ();

            // ToDo: Only first Touch?
            // Handle only first Touch
            if (!touches.Any ()) return;

            var first = touches.First ();

            if (first.State == TouchLocationState.Pressed) 
                        HandleTouch (first.Position);

//            // Process touch locations
//            foreach (TouchLocation location in curTouches)
//            {
//                switch (location.State)
//                {
//                    case TouchLocationState.Pressed:
//                        HandleTouch (location.Position);
//                        break;
//                    case TouchLocationState.Released:
//                        break;
//                    case TouchLocationState.Moved:
//                        break;
//                }
//            }
        }

        //------------------------------------------------------------------
        private void HandleTouch (Vector2 position)
        {
            // ToDo: Width is hardcoded now
            int halfWidth = 240;

            ChangeLane (position.X < halfWidth ? player.Car.Lane.Left : player.Car.Lane.Right);
        }

        //------------------------------------------------------------------
        protected void ForceAccelerate ()
        {
            player.Car.Velocity += player.Car.Acceleration;
        }

        //------------------------------------------------------------------
        private void ChangeLane (Lane lane)
        {
            player.ChangeLane (this, lane, player.GetChangeLanesDuration ());
        }
    }
}