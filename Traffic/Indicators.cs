using Engine;
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
            Fixed = true;
        }

        //------------------------------------------------------------------
        public override void Setup (Game game)
        {
            base.Setup (game);

            font = road.Game.Content.Load <SpriteFont> ("Fonts/Segoe (UI)");
            brains = road.Game.Content.Load <Texture2D> ("Images/Road/Brain");
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch batch)
        {
            var offset = new Vector2 (0, 30);

            batch.DrawString (font, System.Math.Floor (road.Player.Velocity).ToString (), Position, Color.CadetBlue);
            batch.DrawString (font, road.Player.Lives.ToString (), Position + offset * 1, Color.DarkRed);
            batch.Draw (brains, new Vector2(380, 10), Color.White);
        }
    }
}