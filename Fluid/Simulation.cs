using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fluid
{
    public class Simulation : DrawableGameComponent
    {
        //------------------------------------------------------------------
        private static class Parameters
        {
            public const int Iterations = 20;
            public const int Size = 256;
            public const float Dt = 1.0f;
            public const float VelocityDiffusion = 1.0f; //0.99f;
            public const float DensityDiffusion = 1.0f; //0.9999f;
            public const float VorticityScale = 0.20f;
        }

        //------------------------------------------------------------------
        private readonly SpriteBatch spriteBatch;

        // Render Targets
        public RenderTarget2D InputVelocities, InputDensities;
        public RenderTarget2D Velocity, Density, Vorticity;
        public RenderTarget2D Divergence, Pressure;
        public RenderTarget2D Temporary, TemporaryHelper;
        public RenderTarget2D FinalRender;
        public Texture2D Boundaries, VelocityOffsetsTable, PressureOffsetsTable;
        public RenderTarget2D VelocityOffsets, PressureOffsets;

        // Spritebatch settings
        public SpriteSortMode SortMode = SpriteSortMode.Immediate;
        public BlendState Blending = BlendState.Opaque;
        public SamplerState Sampling = new SamplerState();
        public SurfaceFormat ColorFormat = SurfaceFormat.HdrBlendable;
        public DepthFormat ZFormat = DepthFormat.None;

        // Fluid shader and parameteres
        public Effect Shader;

        // Effect Parameters        
        private EffectParameter pVelocitySrc;
        private EffectParameter pDensitySrc;
        private EffectParameter pSplatColor;
        private Texture2D brush;


        // Color splatted for the current sprite during the VelocitySplat pass        
        //------------------------------------------------------------------
        public Vector4 SplatColor
        {
            get { return pSplatColor.GetValueVector4(); }
            set { pSplatColor.SetValue (value); }
        }

        //------------------------------------------------------------------
        public Simulation (Game game) : base (game)
        {
            // Get the content manager and graphics device from the creating entity
            Shader = Game.Content.Load <Effect> ("Fluid/Fluid");
            spriteBatch = new SpriteBatch (GraphicsDevice);
            DrawOrder = 3;

            // Setup EffectParameters
            pSplatColor = Shader.Parameters["VelocityColor"];
            pVelocitySrc = Shader.Parameters["VelocitySrc"];
            pDensitySrc = Shader.Parameters["DensitySrc"];

            // Setup SamplerStates
            Sampling.AddressU = TextureAddressMode.Clamp;
            Sampling.AddressV = TextureAddressMode.Clamp;
            Sampling.AddressW = TextureAddressMode.Clamp;
            Sampling.Filter = TextureFilter.Point;
            for (int i = 0; i < 16; i++)
                GraphicsDevice.SamplerStates[i] = Sampling;

            // Setup fluid parameters
            Shader.Parameters["FluidSize"].SetValue ((float) Parameters.Size);
            Shader.Parameters["VelocityDiffusion"].SetValue (Parameters.VelocityDiffusion);
            Shader.Parameters["DensityDiffusion"].SetValue (Parameters.DensityDiffusion);
            Shader.Parameters["VorticityScale"].SetValue (Parameters.VorticityScale);
            Shader.Parameters["dT"].SetValue (Parameters.Dt);
            Shader.Parameters["HalfCellSize"].SetValue (0.5f);

            // Setup RenderTarget2D's
            Viewport viewport = GraphicsDevice.Viewport;

            InputVelocities = new RenderTarget2D (GraphicsDevice, (int) viewport.Width, (int) viewport.Height, false, ColorFormat, ZFormat);
            InputDensities = new RenderTarget2D (GraphicsDevice, (int) viewport.Width, (int) viewport.Height, false, ColorFormat, ZFormat);

            Velocity = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);
            Density = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);
            Vorticity = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);
            Pressure = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);
            Divergence = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);

            Temporary = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);
            TemporaryHelper = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, ColorFormat, ZFormat);

            FinalRender = new RenderTarget2D (GraphicsDevice, viewport.Width, viewport.Height, false, ColorFormat, ZFormat);

            Boundaries = Game.Content.Load <Texture2D> ("Fluid/Boundaries");
            VelocityOffsets = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false,
                ColorFormat, ZFormat);
            PressureOffsets = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false,
                ColorFormat, ZFormat);
            CreateOffsetTables (GraphicsDevice);
            Shader.Parameters["VelocityOffsets"].SetValue (VelocityOffsets);
            Shader.Parameters["PressureOffsets"].SetValue (PressureOffsets);

            brush = Game.Content.Load <Texture2D> ("Fluid/brush");
        }

        //------------------------------------------------------------------
        private void CreateOffsetTables (GraphicsDevice graphics)
        {
            float[] velocityData = new float[136]
            {
                // This cell is a fluid cell
                1, 0, 1, 0, // Free (no neighboring boundaries)
                0, 0, -1, 1, // East (a boundary to the east)
                1, 0, 1, 0, // Unused
                1, 0, 0, 0, // North
                0, 0, 0, 0, // Northeast
                1, 0, 1, 0, // South
                0, 0, 1, 0, // Southeast
                1, 0, 1, 0, // West
                1, 0, 1, 0, // Unused
                0, 0, 0, 0, // surrounded (3 neighbors)
                1, 0, 0, 0, // Northwest
                0, 0, 0, 0, // surrounded (3 neighbors)
                1, 0, 1, 0, // Southwest 
                0, 0, 0, 0, // surrounded (3 neighbors)
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // surrounded (3 neighbors)
                0, 0, 0, 0, // surrounded (4 neighbors)
                // This cell is a boundary cell (the inverse of above!)
                1, 0, 1, 0, // No neighboring boundaries (Error)
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // Unused
                -1, -1, -1, -1, // Southwest 
                0, 0, 0, 0, // Unused
                -1, 1, 0, 0, // Northwest
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // Unused
                0, 0, -1, -1, // West
                0, 0, -1, 1, // Southeast
                -1, -1, 0, 0, // South
                0, 0, 0, 0, // Northeast
                -1, 1, 0, 0, // North
                0, 0, 0, 0, // Unused
                0, 0, -1, 1, // East (a boundary to the east)
                0, 0, 0, 0 // Unused
            };

            VelocityOffsetsTable = new Texture2D (GraphicsDevice, 34, 1, false, SurfaceFormat.Vector4);
            VelocityOffsetsTable.SetData (velocityData);


            float[] pressureData = new float[136]
            {
                // This cell is a fluid cell
                0, 0, 0, 0, // Free (no neighboring boundaries)
                0, 0, 0, 0, // East (a boundary to the east)
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // North
                0, 0, 0, 0, // Northeast
                0, 0, 0, 0, // South
                0, 0, 0, 0, // Southeast
                0, 0, 0, 0, // West
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // Landlocked (3 neighbors)
                0, 0, 0, 0, // Northwest
                0, 0, 0, 0, // Landlocked (3 neighbors)
                0, 0, 0, 0, // Southwest 
                0, 0, 0, 0, // Landlocked (3 neighbors)
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // Landlocked (3 neighbors)
                0, 0, 0, 0, // Landlocked (4 neighbors)
                // This cell is a boundary cell (the inverse of above!)
                0, 0, 0, 0, // no neighboring boundaries
                0, 0, 0, 0, // unused
                0, 0, 0, 0, // unused
                0, 0, 0, 0, // unused
                -1, 0, 0, -1, // Southwest 
                0, 0, 0, 0, // unused
                -1, 0, 0, 1, // Northwest
                0, 0, 0, 0, // Unused
                0, 0, 0, 0, // Unused
                -1, 0, -1, 0, // West
                0, -1, 1, 0, // Southeast
                0, -1, 0, -1, // South
                0, 1, 1, 0, // Northeast
                0, 1, 0, 1, // North
                0, 0, 0, 0, // Unused
                1, 0, 1, 0, // East (a boundary to the east)
                0, 0, 0, 0 // Unused
            };

            PressureOffsetsTable = new Texture2D (GraphicsDevice, 34, 1, false, SurfaceFormat.Vector4);
            PressureOffsetsTable.SetData (pressureData);
        }

        //------------------------------------------------------------------
        public void AddVelocity()
        {
            var mouse = Mouse.GetState ();
            var position = new Vector2 (mouse.X, mouse.Y);

            Shader.CurrentTechnique = Shader.Techniques["VelocityColorize"];
            GraphicsDevice.SetRenderTarget (InputVelocities);
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (SortMode, BlendState.AlphaBlend, Sampling, null, null, Shader);
            spriteBatch.Draw (brush, position, null, Color.White, 0.0f, new Vector2 (32.0f, 32.0f), 1, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public void AddDensity()
        {
            var mouse = Mouse.GetState ();
            var position = new Vector2 (mouse.X, mouse.Y);
            SplatColor = new Vector4 (350, 0, 0, 1);

            GraphicsDevice.SetRenderTarget (InputDensities);
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (SortMode, BlendState.AlphaBlend, Sampling, null, null);
            spriteBatch.Draw (brush, position, null, Color.White, 0.0f, new Vector2 (32.0f, 32.0f), 1, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public void Update()
        {
            AddSplats();
            UpdateOffsets();

            DoAdvect();
            DoDivergence();
            DoPressure();
            DoSubstract();
            DoVorticity();

            Render();
        }

        //------------------------------------------------------------------
        private void AddSplats()
        {
            AddVelocity();
            AddDensity();

            // Add the velocity and density splats into the fluid
            Shader.CurrentTechnique = Shader.Techniques["DoAddSources"];
            GraphicsDevice.SetRenderTargets (Temporary, TemporaryHelper);
            GraphicsDevice.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Density"].SetValue (Density);
            Shader.Parameters["VelocitySources"].SetValue (InputVelocities);
            Shader.Parameters["DensitySources"].SetValue (InputDensities);

            Draw (Velocity);
            Copy (Temporary, Velocity);
            Copy (TemporaryHelper, Density);

            DoVelocityBoundaries();
        }

        //------------------------------------------------------------------
        private void DoAdvect()
        {
            // Advect the velocity and density
            // Other quantities can be advected too
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            GraphicsDevice.SetRenderTargets (Temporary, TemporaryHelper);
            GraphicsDevice.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Density"].SetValue (Density);

            Draw (Velocity);
            Copy (Temporary, Velocity);
            Copy (TemporaryHelper, Density);

            DoVelocityBoundaries();
        }

        //------------------------------------------------------------------
        private void DoDivergence()
        {
            // Calculate the divergence of the velocity
            Shader.CurrentTechnique = Shader.Techniques["DoDivergence"];
            GraphicsDevice.SetRenderTarget (Divergence);
            GraphicsDevice.Clear (Color.Black);
            Draw (Velocity);

            // Update the shaders copy of the divergence
            GraphicsDevice.SetRenderTarget (null);
            Shader.Parameters["Divergence"].SetValue (Divergence);
        }

        //------------------------------------------------------------------
        private void DoPressure()
        {
            // Iterate over the grid calculating the pressure
            Shader.CurrentTechnique = Shader.Techniques["DoJacobi"];

            // Clear Temp
            GraphicsDevice.SetRenderTarget (Temporary);
            GraphicsDevice.Clear (Color.Black);

            for (int i = 0; i < Parameters.Iterations; i++)
            {
                GraphicsDevice.SetRenderTarget (Pressure);
                GraphicsDevice.Clear (Color.Black);
                Draw (Temporary);

                DoPressureBoundaries (Pressure);

                GraphicsDevice.SetRenderTarget (Temporary);
                GraphicsDevice.Clear (Color.Black);
                Draw (Pressure);
            }

            Copy (Temporary, Pressure);

            // Update the shaders copy of the pressure
            GraphicsDevice.SetRenderTarget (null);
            Shader.Parameters["Pressure"].SetValue (Pressure);
        }

        //------------------------------------------------------------------
        private void DoSubstract()
        {
            // Subtract the pressure from the velocity
            Shader.CurrentTechnique = Shader.Techniques["Subtract"];
            GraphicsDevice.SetRenderTarget (Temporary);
            GraphicsDevice.Clear (Color.Black);
            Draw (Velocity);
            Copy (Temporary, Velocity);
        }

        //------------------------------------------------------------------
        private void DoVorticity()
        {
            // Calculate Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticity"];
            GraphicsDevice.SetRenderTarget (Vorticity);
            GraphicsDevice.Clear (Color.Black);
            Draw (Velocity);

            // Apply Force to Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticityForce"];
            GraphicsDevice.SetRenderTarget (Temporary);
            Shader.Parameters["Vorticity"].SetValue (Vorticity);
            GraphicsDevice.Clear (Color.Black);
            Draw (Vorticity);
            Copy (Temporary, Velocity);
        }

        //-----------------------------------------------------------------
        private void DoVelocityBoundaries()
        {
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryVelocityBoundary"];

            // Update Velocity Offsets
            GraphicsDevice.SetRenderTarget (Temporary);
            GraphicsDevice.Clear (Color.Black);
            Draw (Velocity);
            Copy (Temporary, Velocity);
        }

        //-----------------------------------------------------------------
        private void DoPressureBoundaries (RenderTarget2D pressure)
        {
            var backup = Shader.CurrentTechnique;
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryPressureBoundary"];

            // Update Velocity Offsets
            GraphicsDevice.SetRenderTarget (TemporaryHelper);
            GraphicsDevice.Clear (Color.Black);
            Draw (pressure);
            Copy (TemporaryHelper, pressure);

            Shader.CurrentTechnique = backup;
        }


        //------------------------------------------------------------------
        private void UpdateOffsets()
        {
            Shader.CurrentTechnique = Shader.Techniques["UpdateOffsets"];

            // Update Velocity Offsets
            GraphicsDevice.SetRenderTarget (VelocityOffsets);
            Shader.Parameters["OffsetTable"].SetValue (VelocityOffsetsTable);
            GraphicsDevice.Clear (Color.Black);
            Draw (Boundaries);

            // Update Pressure Offsets
            GraphicsDevice.SetRenderTarget (PressureOffsets);
            Shader.Parameters["OffsetTable"].SetValue (PressureOffsetsTable);
            GraphicsDevice.Clear (Color.Black);
            Draw (Boundaries);
        }

        //------------------------------------------------------------------
        private void Render()
        {
            Viewport viewport = GraphicsDevice.Viewport;

            // Smooth and render the fluid to the screen            
            Shader.CurrentTechnique = Shader.Techniques["Final"];
            GraphicsDevice.SetRenderTarget (FinalRender);
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (SortMode, Blending, Sampling, null, null, Shader);
            spriteBatch.Draw (Density, new Rectangle (0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        private void Copy (RenderTarget2D source, RenderTarget2D destination)
        {
            GraphicsDevice.SetRenderTarget (destination);
            GraphicsDevice.Clear (Color.Black);

            spriteBatch.Begin (SortMode, Blending, Sampling, null, null);
            spriteBatch.Draw (source, Vector2.Zero, Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        private void Draw (Texture2D texture)
        {
            spriteBatch.Begin (SortMode, Blending, Sampling, null, null, Shader);
            spriteBatch.Draw (texture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            // Compute Shaders
            Update();

            // Draw Final image
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null, null);
            spriteBatch.Draw (Traffic.Manager.Scene, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw (FinalRender, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            base.Draw (gameTime);
        }
    }
}