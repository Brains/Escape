#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

            Window.SetPosition (new Point (600, 125));
        }

        //------------------------------------------------------------------
        protected override void Initialize ( )
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
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();

            ControlTimeScale();
            
            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        private void ControlTimeScale()
        {
            float scale = 0.05f;

            if (Traffic.Actions.Input.IsKeyPressed (Keys.D1))
                Traffic.Settings.TimeScale -= scale;
            if (Traffic.Actions.Input.IsKeyPressed (Keys.D2))
                Traffic.Settings.TimeScale += scale;
        }

        //------------------------------------------------------------------
        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.White);

            base.Draw (gameTime);
        }
    }
}
