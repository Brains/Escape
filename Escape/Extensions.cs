using System;
using Microsoft.Xna.Framework;

namespace Escape
{
    public static class WindowExtensions
    {
        //------------------------------------------------------------------
        public static void SetPosition (this GameWindow window, Point position)
        {
            OpenTK.GameWindow otkWindow = GetForm (window);
            if (otkWindow != null)
            {
                otkWindow.X = position.X;
                otkWindow.Y = position.Y;
            }
        }

        //------------------------------------------------------------------
        public static OpenTK.GameWindow GetForm (this GameWindow gameWindow)
        {
            Type type = typeof (OpenTK.GameWindow);
            System.Reflection.FieldInfo field = type.GetField ("window",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                return field.GetValue (gameWindow) as OpenTK.GameWindow;
            return null;
        }
    }
}