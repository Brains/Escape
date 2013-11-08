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
            font = Manager.Instance.Game.Content.Load<SpriteFont> ("Fonts/Segoe (Markers)");
        }

        //------------------------------------------------------------------
        public Text (string text, Vector2 position)
        {
            Position = position;
            String = text;
            Color = Color.DarkCyan;
        }

        //------------------------------------------------------------------
        public Text (string text, Vector2 position, Color color, bool shift = false) : this (text, position)
        {
            this.Color = color;

            if (shift) 
                Position += new Vector2 (20, 0);
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString (font, String, Position, Color);
        }
    }
}