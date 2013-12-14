using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    public class Perfomance : DrawableGameComponent
    {
        private SpriteFont font;
        private int totalFrames = 0;
        private float elapsedTime = 0.0f;
        private int fps = 0;
        private readonly SpriteBatch spriteBatch;

        //------------------------------------------------------------------
        public Perfomance (Game game) : base (game)
        {
            spriteBatch = new SpriteBatch (GraphicsDevice); ;
            font = Game.Content.Load <SpriteFont> ("Fonts/Segoe (UI)");
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            // Update
            elapsedTime += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            // Only update total frames when drawing
            totalFrames++;

            spriteBatch.Begin();
            spriteBatch.DrawString (font, string.Format ("{0}", fps), new Vector2 (420.0f, 20.0f), Color.Gray);
            spriteBatch.End();
        }
    }
}