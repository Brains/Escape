#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Physics;
using Tools;

#endregion

namespace Application
{
    public class Escape : Game
    {
        GraphicsDeviceManager graphics;

        //------------------------------------------------------------------
        public Escape ( )
        {
            graphics = new GraphicsDeviceManager (this);
//            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            
            Content.RootDirectory = "Content";

            Components.Add (new Traffic.Manager (this));
            Components.Add (new Physics.KeyboardInput (this));
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
