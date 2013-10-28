using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tools.Markers
{
    public class Manager : DrawableGameComponent
    {
        public static List<Marker> Markers { get; set; }
        public static Manager Instance { get; set; }
        public static bool Clear { get; set; }
        
        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
            Markers = new List<Marker> ();
            Clear = true;
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            if (Keyboard.GetState ().IsKeyDown (Keys.Space))
                Clear = true;
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            var spriteBatch = new SpriteBatch (Game.GraphicsDevice);

            spriteBatch.Begin ();

            foreach (var marker in Markers)
            {
                marker.Draw (spriteBatch);
            }

            spriteBatch.End ();

            if (Clear)
                Markers.Clear ();
        }
    }
}