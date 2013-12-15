#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Escape
{
    public class EscapeGame : Game
    {
        //------------------------------------------------------------------
        public EscapeGame ()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager (this);

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            Content.RootDirectory = "Content";
        }

        //------------------------------------------------------------------
        protected override void Initialize ()
        {
            Components.Add (new Traffic.Actions.Base.Manager (this));
            Components.Add (new Tools.Timers.Manager (this));
            Components.Add (new Traffic.Manager (this));
            Components.Add (new Tools.Markers.Manager (this));

            base.Initialize ();
        }

        //------------------------------------------------------------------
        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();

            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.White);

            base.Draw (gameTime);
        }
    }
}