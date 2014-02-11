using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Android
{
    public class Escape : Microsoft.Xna.Framework.Game
    {
        SpriteBatch Batch;
        protected RenderTarget2D Output;
        protected Texture2D Texture;
        protected Effect Shader;

        //------------------------------------------------------------------
        public Escape ()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager (this);
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            // Disable fixed framerate
//            graphics.SynchronizeWithVerticalRetrace = false;
//            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
        }

        //------------------------------------------------------------------
        protected override void Initialize ()
        {
//            Components.Add (new Traffic.Actions.Base.Manager (this));
//            Components.Add (new Tools.Timers.Manager (this));
//            Components.Add (new Traffic.Manager (this));
//            Components.Add (new Tools.Markers.Manager (this));
//            Components.Add (new Tools.Perfomance (this));
            
            Shader = Content.Load<Effect> ("Fluid/Test");

            Batch = new SpriteBatch (GraphicsDevice);

            Output = new RenderTarget2D (GraphicsDevice, 256, 256);
            FillTexture(Output, Color.GreenYellow);
            
            Texture = new Texture2D (GraphicsDevice, 128, 128);
            FillTexture (Texture, Color.SlateBlue);

//            base.Initialize ();
        }

        //------------------------------------------------------------------
        private static void FillTexture (Texture2D renderTarget2D, Color greenYellow)
        {
            Color[] colors2D = new Color[renderTarget2D.Width * renderTarget2D.Height];

            for (int x = 0; x < colors2D.Length; x++)
                colors2D[x] = greenYellow;

            renderTarget2D.SetData (colors2D);
        }

        //------------------------------------------------------------------
        protected override void Update (GameTime gameTime)
        {
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
                Exit ();

//            ControlTimeScale ();

//            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        private void ControlTimeScale ()
        {
            float scale = 0.05f;

            var pressed = Keyboard.GetState ().GetPressedKeys ();

            if (pressed.Contains(Keys.D1))
                Traffic.Settings.TimeScale -= scale;
            if (pressed.Contains (Keys.D2))
                Traffic.Settings.TimeScale += scale;
        }

        //------------------------------------------------------------------
        protected override void Draw (GameTime gameTime)
        {
            // Render into Output
            Shader.CurrentTechnique = Shader.Techniques["Test"];
            GraphicsDevice.SetRenderTarget (Output);
            GraphicsDevice.Clear (Color.DimGray);

            Batch.Begin (SpriteSortMode.BackToFront, BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.None, null, Shader);
//            Batch.Begin ();
            Batch.Draw (Texture, Vector2.Zero, Color.White);
            Batch.End ();

            GraphicsDevice.SetRenderTarget (null);


            // Draw Output
            GraphicsDevice.Clear (Color.White);

            Batch.Begin ();
            Batch.Draw (Output, Vector2.Zero, Color.White);
            Batch.End ();
        }
    }
}