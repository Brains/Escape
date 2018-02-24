using System.Collections.Generic;
using Engine.Tools;
using Engine.Tools.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Escape
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        //------------------------------------------------------------------
        public Game()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager (this);

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            graphics.SupportedOrientations = DisplayOrientation.Portrait;


            Content.RootDirectory = "Content";

//            DisableFixedFramerate (graphics);
            IsMouseVisible = true;
         
        }

        //------------------------------------------------------------------
        private void DisableFixedFramerate (GraphicsDeviceManager graphics)
        {
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        //------------------------------------------------------------------
        protected override void Initialize()
        {
            Components.Add (new Engine.Actions.Manager (this));
            Components.Add (new Manager (this));
            Components.Add (new Traffic.Manager (this));
            Components.Add (new Engine.Tools.Markers.Manager (this));
            Components.Add (new Perfomance (this));

            base.Initialize();
        }

        //------------------------------------------------------------------
        protected override void Update (GameTime gameTime)
        {
    
            ControlTimeScale();

            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        private void ControlTimeScale()
        {
            float scale = 0.05f;

            if (Keyboard.GetState().IsKeyDown (Keys.D1))
                Traffic.Settings.TimeScale -= scale;
            if (Keyboard.GetState().IsKeyDown (Keys.D2))
                Traffic.Settings.TimeScale += scale;
        }

        //------------------------------------------------------------------
        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.HotPink);

            base.Draw (gameTime);
        }
    }
}