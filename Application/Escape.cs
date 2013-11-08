#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Traffic.Actions.Base;

#endregion

namespace Application
{
    public class Escape : Game
    {
        //------------------------------------------------------------------
        public Escape ( )
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager (this);
//            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            
            Content.RootDirectory = "Content";

            Components.Add (new Manager (this));
            Components.Add (new Tools.Timers.Manager (this));
            Components.Add (new Traffic.Manager (this));
            Components.Add (new Tools.Markers.Manager (this));
        }

        //------------------------------------------------------------------
        protected override void Initialize ( )
        {
            base.Initialize ();
        }

        //------------------------------------------------------------------
        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
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
