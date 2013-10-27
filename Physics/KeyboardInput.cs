using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Physics
{
	public class KeyboardInput : GameComponent
    {
	    //------------------------------------------------------------------
        private Vector2 shift;
	    private static KeyboardState current;
	    private static KeyboardState previous;

	    //------------------------------------------------------------------
        public Vector2 Shift
        {
            get { return shift; }
            set { shift = value; }
        }

	    //------------------------------------------------------------------
        public KeyboardInput (Game game) : base (game)
	    {
	        
	    }

	    //------------------------------------------------------------------
	    public override void Update (GameTime gameTime)
	    {
	        previous = current;

            current = Keyboard.GetState ();
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
    }
}