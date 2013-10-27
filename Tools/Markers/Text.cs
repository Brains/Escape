using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools.Markers
{
    public class Text : Marker
    {
        private static SpriteFont font;
        
        public string String { get; set; }

        //------------------------------------------------------------------
        static Text ()
        {
            font = Manager.Game.Content.Load <SpriteFont> ("Fonts/Segoe UI Light");
        }

        //------------------------------------------------------------------
        public Text (string text, Vector2 position)
        {
            Position = position;
            String = text;
            Color = Color.DarkCyan;

            Manager.Markers.Add (this);
        }

        //------------------------------------------------------------------
        public Text (string text, Vector2 position, Color color) : this (text, position)
        {
            this.Color = color;
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString (font, String, Position, Color);
        }
    }
}