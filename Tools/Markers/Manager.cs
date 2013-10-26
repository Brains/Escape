using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Markers
{
    public class Manager
    {
        static public List<Marker> Markers { get; set; }
        static public Game Game { get; set; }

        //------------------------------------------------------------------
        static Manager ()
        {
            Markers = new List<Marker> ();
        }

        //------------------------------------------------------------------
        public static void DrawAllMarkers (Game game)
        {
            var spriteBatch = new SpriteBatch (game.GraphicsDevice);

            spriteBatch.Begin ();

            foreach (var marker in Markers)
            {
                marker.Draw (spriteBatch);
            }

            spriteBatch.End ();

            Markers.Clear ();
        }
    }
}