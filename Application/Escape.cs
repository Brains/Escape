using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Application
{
    public class Escape : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        //------------------------------------------------------------------
        public Escape ( )
        {
            graphics = new GraphicsDeviceManager (this);

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        //------------------------------------------------------------------
        protected override void Initialize ( )
        {
            Services.AddService (typeof (ContentManager), Content);

            base.Initialize ();
        }

        //------------------------------------------------------------------
        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit ();

            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        protected override void Draw (GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear (Color.White);

            base.Draw (gameTime);
        }
    }
}
