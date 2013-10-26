#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

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
        }

        //------------------------------------------------------------------
        protected override void Initialize ( )
        {
            Services.AddService (typeof (ContentManager), Content);

            Tools.Markers.Manager.Game = this;

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

            Tools.Markers.Manager.DrawAllMarkers (this);
        }
    }
}
