/*
    Fluid Shader Demo - Demo of an HLSL fluid shader
    Copyright (C) 2013 Michael Stone (Neoaikon)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fluid
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D brush;
        private FluidParams fluidParams = new FluidParams();
        private Simulation simulation;

        private float brushSize = 128.0f;
        private Color brushColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        private Vector4 velocityColor = new Vector4();

        private MouseState ms;
        private Vector2 mouse, lastMouse;
        private Processor processor;

        public Game()
        {
            graphics = new GraphicsDeviceManager (this);
            Content.RootDirectory = "Content";
        }

        //------------------------------------------------------------------
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        //------------------------------------------------------------------
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch (GraphicsDevice);

            brush = Content.Load <Texture2D> ("brush");

            fluidParams.Iterations = 20;
            fluidParams.GridSize = 256;
            fluidParams.ScreenSize = new Vector2 (1000, 1000);
            // Don't set these greater than 1.0f!
            fluidParams.VelocityDiffusion = .99f;
            fluidParams.DensityDiffusion = .9999f;

            simulation = new Simulation (Content, graphics.GraphicsDevice, spriteBatch, fluidParams);
            processor = new Processor (simulation, GraphicsDevice, Content);

            Components.Add (new Perfomance (this, Content, spriteBatch));
        }

        //------------------------------------------------------------------
        protected override void UnloadContent()
        {
            simulation = null;
            fluidParams = null;
            brush.Dispose();
        }

        //------------------------------------------------------------------
        protected override void Update (GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown (Keys.Escape))
                this.Exit();

            ms = Mouse.GetState();
            mouse = new Vector2 (ms.X, ms.Y);
            velocityColor = new Vector4 (lastMouse - mouse, 0.0f, 1.0f);
            lastMouse = mouse;

            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        protected override void Draw (GameTime gameTime)
        {
            simulation.BeginDensityPass();
            if (ms.LeftButton == ButtonState.Pressed)
                spriteBatch.Draw(brush, mouse, null, brushColor, 0.0f, new Vector2(32.0f, 32.0f), brushSize / 16.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw (brush, mouse, null, Color.White, 0.0f, new Vector2 (32.0f, 32.0f), 1, SpriteEffects.None, 0.0f);
            simulation.EndPass();

            simulation.BeginVelocityPass();

            simulation.SplatColor = new Vector4 (350, 0, 0, 1); //velocityColor;
            spriteBatch.Draw (brush, mouse, null, Color.White, 0.0f, new Vector2 (32.0f, 32.0f), 2, SpriteEffects.None, 0.0f);

//            if (ms.RightButton == ButtonState.Pressed)
//            {
//                Simulation.SplatColor = velocityColor;
//                spriteBatch.Draw(brush, mouse, null, Color.White, 0.0f, new Vector2(32.0f, 32.0f), brushSize/64.0f, SpriteEffects.None, 0.0f);
//            }
            simulation.EndPass();

            simulation.Update();

            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            simulation.Draw (Vector2.Zero);
            spriteBatch.End();


//            processor.Analyze (spriteBatch);

            base.Draw (gameTime);
        }
    }
}