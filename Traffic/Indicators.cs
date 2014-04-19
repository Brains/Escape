using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    internal class Indicators : Object
    {
        private Road road;
        private SpriteFont font;
        private Texture2D brains;

        //------------------------------------------------------------------
        public Indicators (Road road) : base (road)
        {
            this.road = road;
            LocalPosition = new Vector2 (10);
            Anchored = true;
        }

        //------------------------------------------------------------------
        public override void Setup ()
        {
            base.Setup ();

            font = road.Game.Content.Load <SpriteFont> ("Fonts/Segoe (UI)");
            brains = road.Game.Content.Load <Texture2D> ("Images/Road/Brain");
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            var offset = new Vector2 (0, 30);

            spriteBatch.DrawString (font, System.Math.Floor (road.Player.Velocity).ToString (), Position, Color.CadetBlue);
            spriteBatch.DrawString (font, road.Player.Lives.ToString (), Position + offset * 1, Color.DarkRed);
            spriteBatch.Draw (brains, new Vector2(380, 10), Color.White);
        }
    }
}