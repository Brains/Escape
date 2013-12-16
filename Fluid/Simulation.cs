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
            public const float VelocityDiffusion = 0.99f;
            public const float DensityDiffusion = 0.999f;
            public const float VorticityScale = 0.30f;
        }

        //------------------------------------------------------------------
        // Render Targets
        #region Textures

        RenderTarget2D inputVelocities, inputDensities;
        RenderTarget2D velocity, density;
        RenderTarget2D divergence, pressure, vorticity;
        RenderTarget2D temporary, temporaryHelper;
        RenderTarget2D final;
        Texture2D velocityOffsetsTable, pressureOffsetsTable;
        RenderTarget2D boundaries, velocityOffsets, pressureOffsets;

        #endregion

        #region Spritebatch settings

        private const SpriteSortMode Sorting = SpriteSortMode.Immediate;
        private readonly BlendState blending = BlendState.Opaque;
        private readonly SamplerState sampling = new SamplerState();
        private const SurfaceFormat Surface = SurfaceFormat.HdrBlendable;
        private const DepthFormat ZFormat = DepthFormat.None;

        #endregion


        // Fluid shader and parameteres
        public Effect Shader;
        private readonly SpriteBatch spriteBatch;

        // Effect Parameters        
        private Texture2D brush;

        //------------------------------------------------------------------
        // Underlie Scene
        public RenderTarget2D Scene { get; set; }

        //------------------------------------------------------------------
        public Simulation (Game game) : base (game)
        {
            Shader = Game.Content.Load <Effect> ("Fluid/Fluid");
            spriteBatch = new SpriteBatch (GraphicsDevice);
            DrawOrder = 3;
            
            // Setup fluid parameters
            Shader.Parameters["FluidSize"].SetValue ((float) Parameters.Size);
            Shader.Parameters["VelocityDiffusion"].SetValue (Parameters.VelocityDiffusion);
            Shader.Parameters["DensityDiffusion"].SetValue (Parameters.DensityDiffusion);
            Shader.Parameters["VorticityScale"].SetValue (Parameters.VorticityScale);
            Shader.Parameters["dT"].SetValue (Parameters.Dt);
            Shader.Parameters["HalfCellSize"].SetValue (0.5f);

            CreateTextures();

            // Setup SamplerStates
            sampling.AddressU = TextureAddressMode.Clamp;
            sampling.AddressV = TextureAddressMode.Clamp;
            sampling.AddressW = TextureAddressMode.Clamp;
            sampling.Filter = TextureFilter.Point;
            for (int i = 0; i < 16; i++)
                GraphicsDevice.SamplerStates[i] = sampling;
        }

        #region Creation

        //------------------------------------------------------------------
        private void CreateTextures ()
        {
            Viewport viewport = GraphicsDevice.Viewport;

            inputVelocities = new RenderTarget2D (GraphicsDevice, (int) viewport.Width, (int) viewport.Height, false, Surface, ZFormat);
            inputDensities = new RenderTarget2D (GraphicsDevice, (int) viewport.Width, (int) viewport.Height, false, Surface, ZFormat);

            velocity = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            density = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            vorticity = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            pressure = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            divergence = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);

            temporary = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            temporaryHelper = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);

            final = new RenderTarget2D (GraphicsDevice, viewport.Width, viewport.Height, false, Surface, ZFormat);

            // Boundaries
            boundaries = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            velocityOffsets = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            pressureOffsets = new RenderTarget2D (GraphicsDevice, Parameters.Size, Parameters.Size, false, Surface, ZFormat);
            Shader.Parameters["VelocityOffsets"].SetValue (velocityOffsets);
            Shader.Parameters["PressureOffsets"].SetValue (pressureOffsets);
            Shader.Parameters["Boundaries"].SetValue(boundaries);
            CreateVelocityOffsetTable();
            CreatePressureOffsetTable();

            brush = Game.Content.Load<Texture2D> ("Fluid/Brush");
        }

        //------------------------------------------------------------------
        private void CreateVelocityOffsetTable()
        {
            float[] data = new float[136]
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

            velocityOffsetsTable = new Texture2D (GraphicsDevice, 34, 1, false, SurfaceFormat.Vector4);
            velocityOffsetsTable.SetData (data);
        }

        //------------------------------------------------------------------
        private void CreatePressureOffsetTable ()
        {
            float[] data = new float[136]
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

            pressureOffsetsTable = new Texture2D (GraphicsDevice, 34, 1, false, SurfaceFormat.Vector4);
            pressureOffsetsTable.SetData (data);
        }

        #endregion

        //------------------------------------------------------------------
        public void Update()
        {
            UpdateOffsets ();
            AddSplats ();
            ComputeVelocityBoundaries ();
            ComputeDensityBoundaries ();

            ComputeAdvect();
            ComputeVelocityBoundaries ();
            ComputeDivergence ();
            ComputePressure();
            ComputeSubstract();
            ComputeVorticity();

            Render();
        }

        #region Addition

        //------------------------------------------------------------------
        public void AddVelocity ()
        {
            Shader.Parameters["Impulse"].SetValue (new Vector4 (0, -1500, 0, 1));

            var scale = new Vector2 (8, 1);

            Shader.CurrentTechnique = Shader.Techniques["VelocityColorize"];
            GraphicsDevice.SetRenderTarget (inputVelocities);
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (Sorting, BlendState.AlphaBlend, sampling, null, null, Shader);
            spriteBatch.Draw (brush, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.Draw (brush, new Vector2(0, 400), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public void AddDensity ()
        {
            var mouse = Mouse.GetState ();
            var position = new Vector2 (mouse.X, mouse.Y);

            var scale = new Vector2 (2, 1);
            Color color = Color.SlateBlue;
            
            GraphicsDevice.SetRenderTarget (inputDensities);
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (Sorting, BlendState.AlphaBlend, sampling, null, null);
            spriteBatch.Draw (brush, position, null, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.End ();
        }

        //------------------------------------------------------------------
        private void AddSplats ()
        {
            AddVelocity();
            AddDensity();

            // Add the velocity and density splats into the fluid
            Shader.CurrentTechnique = Shader.Techniques["DoAddSources"];
            GraphicsDevice.SetRenderTargets (temporary, temporaryHelper);
            GraphicsDevice.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (velocity);
            Shader.Parameters["Density"].SetValue (density);
            Shader.Parameters["VelocitySources"].SetValue (inputVelocities);
            Shader.Parameters["DensitySources"].SetValue (inputDensities);

            Draw (velocity);
            Copy (temporary, velocity);
            Copy (temporaryHelper, density);

        }

        #endregion

        //------------------------------------------------------------------
        #region Computations

        private void ComputeAdvect()
        {
            // Advect the velocity and density
            // Other quantities can be advected too
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            GraphicsDevice.SetRenderTargets (temporary, temporaryHelper);
            GraphicsDevice.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (velocity);
            Shader.Parameters["Density"].SetValue (density);

            Draw (velocity);
            Copy (temporary, velocity);
            Copy (temporaryHelper, density);

        }

        //------------------------------------------------------------------
        private void ComputeDivergence()
        {
            // Calculate the divergence of the velocity
            Shader.CurrentTechnique = Shader.Techniques["DoDivergence"];
            GraphicsDevice.SetRenderTarget (divergence);
            GraphicsDevice.Clear (Color.Black);
            Draw (velocity);

            // Update the shaders copy of the divergence
            GraphicsDevice.SetRenderTarget (null);
            Shader.Parameters["Divergence"].SetValue (divergence);
        }

        //------------------------------------------------------------------
        private void ComputePressure()
        {
            // Iterate over the grid calculating the pressure
            Shader.CurrentTechnique = Shader.Techniques["DoJacobi"];

            // Clear Temp
            GraphicsDevice.SetRenderTarget (temporary);
            GraphicsDevice.Clear (Color.Black);

            for (int i = 0; i < Parameters.Iterations; i++)
            {
                GraphicsDevice.SetRenderTarget (pressure);
                GraphicsDevice.Clear (Color.Black);
                Draw (temporary);

                ComputePressureBoundaries (pressure);

                GraphicsDevice.SetRenderTarget (temporary);
                GraphicsDevice.Clear (Color.Black);
                Draw (pressure);
            }

            Copy (temporary, pressure);

            // Update the shaders copy of the pressure
            GraphicsDevice.SetRenderTarget (null);
            Shader.Parameters["Pressure"].SetValue (pressure);
        }

        //------------------------------------------------------------------
        private void ComputeSubstract()
        {
            // Subtract the pressure from the velocity
            Shader.CurrentTechnique = Shader.Techniques["Subtract"];
            GraphicsDevice.SetRenderTarget (temporary);
            GraphicsDevice.Clear (Color.Black);
            Draw (velocity);
            Copy (temporary, velocity);
        }

        //------------------------------------------------------------------
        private void ComputeVorticity()
        {
            // Calculate Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticity"];
            GraphicsDevice.SetRenderTarget (vorticity);
            GraphicsDevice.Clear (Color.Black);
            Draw (velocity);

            // Apply Force to Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticityForce"];
            GraphicsDevice.SetRenderTarget (temporary);
            Shader.Parameters["Vorticity"].SetValue (vorticity);
            GraphicsDevice.Clear (Color.Black);
            Draw (vorticity);
            Copy (temporary, velocity);
        }

        #endregion

        #region Obstacles

        //------------------------------------------------------------------
        private void UpdateOffsets ()
        {
            ShapeObstacles();

            Shader.CurrentTechnique = Shader.Techniques["UpdateOffsets"];

            // Update Velocity Offsets
            GraphicsDevice.SetRenderTarget (velocityOffsets);
            Shader.Parameters["OffsetTable"].SetValue (velocityOffsetsTable);
            GraphicsDevice.Clear (Color.Black);
            Draw (boundaries);

            // Update Pressure Offsets
            GraphicsDevice.SetRenderTarget (pressureOffsets);
            Shader.Parameters["OffsetTable"].SetValue (pressureOffsetsTable);
            GraphicsDevice.Clear (Color.Black);
            Draw (boundaries);
        }

        //------------------------------------------------------------------
        private void ShapeObstacles ()
        {
            if (Scene == null) return;
            Vector2 scale = new Vector2 ((float) Parameters.Size / Scene.Width, (float) Parameters.Size / Scene.Height);

            Shader.CurrentTechnique = Shader.Techniques["ShapeObstacles"];
            GraphicsDevice.SetRenderTarget (boundaries);
            GraphicsDevice.Clear (Color.Black);

            spriteBatch.Begin (Sorting, blending, sampling, null, null, Shader);
            spriteBatch.Draw (Scene, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.End ();

            GraphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        private void ComputeVelocityBoundaries()
        {
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryVelocityBoundary"];
            GraphicsDevice.SetRenderTarget (temporary);
            GraphicsDevice.Clear (Color.Black);
            Draw (velocity);
            Copy (temporary, velocity);
        }

        //-----------------------------------------------------------------
        private void ComputeDensityBoundaries ()
        {
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryDensityBoundary"];
            GraphicsDevice.SetRenderTarget (temporary);
            GraphicsDevice.Clear (Color.Black);
            Draw (density);
            Copy (temporary, density);
        }

        //-----------------------------------------------------------------
        private void ComputePressureBoundaries (RenderTarget2D pressure)
        {
            var backup = Shader.CurrentTechnique;
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryPressureBoundary"];

            // Update Velocity Offsets
            GraphicsDevice.SetRenderTarget (temporaryHelper);
            GraphicsDevice.Clear (Color.Black);
            Draw (pressure);
            Copy (temporaryHelper, pressure);

            Shader.CurrentTechnique = backup;
        }

        #endregion

        //------------------------------------------------------------------
        private void Copy (RenderTarget2D source, RenderTarget2D destination)
        {
            GraphicsDevice.SetRenderTarget (destination);
            GraphicsDevice.Clear (Color.Black);
            
            spriteBatch.Begin (Sorting, blending, sampling, null, null);
            spriteBatch.Draw (source, Vector2.Zero, Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget (null);
        }

        #region Render

        //------------------------------------------------------------------
        private void Render ()
        {
            Viewport viewport = GraphicsDevice.Viewport;

            // Smooth and render the fluid to the screen            
            Shader.CurrentTechnique = Shader.Techniques["Final"];
            GraphicsDevice.SetRenderTarget (final);
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (Sorting, blending, sampling, null, null, Shader);
            spriteBatch.Draw (density, new Rectangle (0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        private void Draw (Texture2D texture)
        {
            spriteBatch.Begin (Sorting, blending, sampling, null, null, Shader);
            spriteBatch.Draw (texture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            // Compute Shaders
            Update();

            // Draw Final image
            GraphicsDevice.Clear (Color.White);
            spriteBatch.Begin (SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null, null);
//            spriteBatch.Draw (Scene, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw (final, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();

            base.Draw (gameTime);
        }

        #endregion
    }
}