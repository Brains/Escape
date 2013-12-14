using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    public class Perfomance : DrawableGameComponent
    {
        private SpriteFont _spr_font;
        private int _total_frames = 0;
        private float _elapsed_time = 0.0f;
        private int _fps = 0;
        private ContentManager Content;
        private SpriteBatch spriteBatch;

        //------------------------------------------------------------------
        public Perfomance (Microsoft.Xna.Framework.Game game, ContentManager content, SpriteBatch spriteBatch) : base (game)
        {
            Content = content;
            this.spriteBatch = spriteBatch;
            _spr_font = Content.Load <SpriteFont> ("Debug");
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            // Update
            _elapsed_time += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            // Only update total frames when drawing
            _total_frames++;

            spriteBatch.Begin();
            spriteBatch.DrawString (_spr_font, string.Format ("FPS={0}", _fps), new Vector2 (10.0f, 20.0f), Color.White);
            spriteBatch.End();
        }
    }
}