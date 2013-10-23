using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Application
{
    // This is the main type for your game
    public class Escape : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public Escape ( )
        {
            graphics = new GraphicsDeviceManager (this);

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            new Traffic.Manager (this);
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize ( )
        {
            Services.AddService(typeof(ContentManager), Content);


            base.Initialize ();
        }

        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        protected override void LoadContent ( )
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch (GraphicsDevice);

            font = Content.Load<SpriteFont> ("SpriteFont");
        }

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit ();

            base.Update (gameTime);
        }

        /// This is called when the game should draw itself.
        protected override void Draw (GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear (Color.White);

            spriteBatch.Begin ();
            spriteBatch.DrawString (font, "Hello from MonoGame!", new Vector2 (16, 16), Color.White);
            spriteBatch.End ();

            base.Draw (gameTime);
        }
    }
}
