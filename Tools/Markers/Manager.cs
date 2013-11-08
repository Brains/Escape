using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tools.Markers
{
    public class Manager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        public static Manager Instance { get; set; }
        public List<Marker> Markers { get; set; }
        public static bool Clear { get; set; }
        
        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
            Clear = true;

            // Draw Markers after all Draw calls
            DrawOrder = int.MaxValue;

            Markers = new List<Marker> ();

            spriteBatch = new SpriteBatch (Game.GraphicsDevice);
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