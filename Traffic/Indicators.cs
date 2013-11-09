using Java.Lang;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    internal class Indicators : Object
    {
        private Road road;
        private SpriteFont font;

        //------------------------------------------------------------------
        public Indicators (Road road) : base (road)
        {
            this.road = road;
            Position = new Vector2 (10);
            Anchored = true;
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            base.Setup ();

            font = road.Game.Content.Load <SpriteFont> ("Fonts/Segoe (UI)");
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            var offset = new Vector2 (0, 30);

            spriteBatch.DrawString (font, System.Math.Floor (road.Player.Velocity).ToString (), GlobalPosition, Color.CadetBlue);
            spriteBatch.DrawString (font, road.Player.Lives.ToString (), GlobalPosition + offset * 1, Color.DarkRed);
        }
    }
}