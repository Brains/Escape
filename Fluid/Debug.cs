using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fluid
{
    static class Debug
    {
//        public static dynamic Variable;
        private static Vector2 previous;

        //------------------------------------------------------------------
        static Debug ()
        {

//        private SpriteFont font;
//            font = game.Content.Load<SpriteFont> ("Fonts/Segoe (UI)");
            
        }

        //------------------------------------------------------------------
        public static Vector2 Mouse ()
        {
            var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState ();
            var position = new Vector2 (mouse.X, mouse.Y);

            return position;
        }

        //------------------------------------------------------------------
        public static Vector2 Direction ()
        {
            return Mouse() - previous;
        }

        //------------------------------------------------------------------
        public static bool RightMouse ()
        {
            return Microsoft.Xna.Framework.Input.Mouse.GetState ().RightButton == ButtonState.Pressed;
        }

        //------------------------------------------------------------------
        public static bool LeftMouse ()
        {
            return Microsoft.Xna.Framework.Input.Mouse.GetState ().LeftButton == ButtonState.Pressed;
        }

        //------------------------------------------------------------------
        public static void Update ()
        {
            const float step = 0.01f;

//            if (Keyboard.GetState().IsKeyDown (Keys.A))
//                Variable -= step;
//                
//            if (Keyboard.GetState ().IsKeyDown (Keys.D))
//                Variable += step;

            previous = Mouse();
        }

    }

    //------------------------------------------------------------------
    sealed class Ref<T>
    {
        private readonly Action<T> setter;
        public Ref (Action<T> setter)
        {
            this.setter = setter;
        }
        public T Value { set { setter (value); } }
    }
}