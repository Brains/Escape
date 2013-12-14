/*
    Fluid.cs - HLSL fluid shader handler class
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    public class Simulation
    {
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;

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
        public SamplerState Sampling = new SamplerState ();
        public SurfaceFormat ColorFormat = SurfaceFormat.HdrBlendable;
        public DepthFormat ZFormat = DepthFormat.None;        
        
        // Fluid shader and parameteres
        public Effect Shader;
        FluidParams Params;

        // Effect Parameters        
        EffectParameter pVelocitySrc;
        EffectParameter pDensitySrc;        
        EffectParameter pSplatColor;


        // Color splatted for the current sprite during the VelocitySplat pass        
        //------------------------------------------------------------------
        public Vector4 SplatColor
        {
            get { return pSplatColor.GetValueVector4(); }
            set { pSplatColor.SetValue(value); }
        }

        //------------------------------------------------------------------
        public Simulation (ContentManager content, GraphicsDevice graphics, SpriteBatch batch, FluidParams parameters)
        {
            // Get the content manager and graphics device from the creating entity
            Shader               = content.Load<Effect>("Fluid");
            graphicsDevice       = graphics;
            spriteBatch          = batch;

            // Setup EffectParameters
            pSplatColor          = Shader.Parameters["VelocityColor"];
            pVelocitySrc         = Shader.Parameters["VelocitySrc"];
            pDensitySrc          = Shader.Parameters["DensitySrc"];            

            // Setup SamplerStates
            Sampling.AddressU = TextureAddressMode.Clamp;
            Sampling.AddressV = TextureAddressMode.Clamp;
            Sampling.AddressW = TextureAddressMode.Clamp;
            Sampling.Filter = TextureFilter.Point;
            for(int i = 0; i < 16; i++)
                graphicsDevice.SamplerStates[i] = Sampling;

            // Setup fluid parameters
            Params = parameters;
            Shader.Parameters["FluidSize"].SetValue((float)Params.GridSize);
            Shader.Parameters["VelocityDiffusion"].SetValue(Params.VelocityDiffusion);
            Shader.Parameters["DensityDiffusion"].SetValue(Params.DensityDiffusion);
            Shader.Parameters["VorticityScale"].SetValue (Params.VorticityScale);
            Shader.Parameters["dT"].SetValue (1.0f);
            Shader.Parameters["HalfCellSize"].SetValue (0.5f);



            // Setup RenderTarget2D's
            InputVelocities = new RenderTarget2D(graphicsDevice, (int)Params.ScreenSize.X, (int)Params.ScreenSize.Y, false, ColorFormat, ZFormat);
            InputDensities  = new RenderTarget2D(graphicsDevice, (int)Params.ScreenSize.X, (int)Params.ScreenSize.Y, false, ColorFormat, ZFormat);
            
            Velocity        = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            Density         = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            Vorticity       = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            Pressure        = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            Divergence      = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            
            Temporary       = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            TemporaryHelper = new RenderTarget2D(graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            
            FinalRender     = new RenderTarget2D(graphicsDevice, (int)Params.ScreenSize.X, (int)Params.ScreenSize.Y, false, ColorFormat, ZFormat);

            Boundaries = content.Load <Texture2D> ("Boundaries");
            VelocityOffsets = new RenderTarget2D (graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            PressureOffsets = new RenderTarget2D (graphicsDevice, Params.GridSize, Params.GridSize, false, ColorFormat, ZFormat);
            CreateOffsetTables (graphics);

            Shader.Parameters["VelocityOffsets"].SetValue (VelocityOffsets);
            Shader.Parameters["PressureOffsets"].SetValue (PressureOffsets);
        }

        //------------------------------------------------------------------
        private void CreateOffsetTables(GraphicsDevice graphics)
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

            VelocityOffsetsTable = new Texture2D (graphicsDevice, 34, 1, false, SurfaceFormat.Vector4);
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

            PressureOffsetsTable = new Texture2D (graphicsDevice, 34, 1, false, SurfaceFormat.Vector4);
            PressureOffsetsTable.SetData (pressureData);
        }

        // Starts the VelocitySplat pass
        //------------------------------------------------------------------
        public void BeginVelocityPass ()
        {
            Shader.CurrentTechnique = Shader.Techniques["VelocityColorize"];
            graphicsDevice.SetRenderTarget(InputVelocities);
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SortMode, BlendState.AlphaBlend, Sampling, null, null, Shader);
        }

        // Starts the DensitySplat pass
        //------------------------------------------------------------------
        public void BeginDensityPass ()
        {
            graphicsDevice.SetRenderTarget(InputDensities);
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SortMode, BlendState.AlphaBlend, Sampling, null, null);
        }

        // Ends either the VelocitySplat or DensitySplat passes
        //------------------------------------------------------------------
        public void EndPass ()
        {
            spriteBatch.End();
        }

        // Render the fluid to the screen
        //------------------------------------------------------------------
        public void Update ()
        {
            AddSplats ();
            UpdateOffsets ();

            DoAdvect();
            DoDivergence();
            DoPressure();
            DoSubstract();
            DoVorticity ();

            Render();
        }

        //------------------------------------------------------------------
        private void AddSplats ()
        {
            // Add the velocity and density splats into the fluid
            Shader.CurrentTechnique = Shader.Techniques["DoAddSources"];
            graphicsDevice.SetRenderTargets (Temporary, TemporaryHelper);
            graphicsDevice.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Density"].SetValue (Density);
            Shader.Parameters["VelocitySources"].SetValue (InputVelocities);
            Shader.Parameters["DensitySources"].SetValue (InputDensities); 
            
            Draw(Velocity);
            Copy (Temporary, Velocity);
            Copy (TemporaryHelper, Density);

            DoVelocityBoundaries ();
        }

        //------------------------------------------------------------------
        private void DoAdvect ()
        {
            // Advect the velocity and density
            // Other quantities can be advected too
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            graphicsDevice.SetRenderTargets (Temporary, TemporaryHelper);
            graphicsDevice.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Density"].SetValue (Density);
            
            Draw (Velocity);
            Copy (Temporary, Velocity);
            Copy (TemporaryHelper, Density);

            DoVelocityBoundaries ();

        }

        //------------------------------------------------------------------
        private void DoDivergence ()
        {
            // Calculate the divergence of the velocity
            Shader.CurrentTechnique = Shader.Techniques["DoDivergence"];
            graphicsDevice.SetRenderTarget (Divergence);
            graphicsDevice.Clear (Color.Black);
            Draw (Velocity);

            // Update the shaders copy of the divergence
            graphicsDevice.SetRenderTarget (null);
            Shader.Parameters["Divergence"].SetValue (Divergence);
        }

        //------------------------------------------------------------------
        private void DoPressure ()
        {
            // Iterate over the grid calculating the pressure
            Shader.CurrentTechnique = Shader.Techniques["DoJacobi"];

            // Clear Temp
            graphicsDevice.SetRenderTarget (Temporary);
            graphicsDevice.Clear (Color.Black);

            for (int i = 0; i < Params.Iterations; i++)
            {
                graphicsDevice.SetRenderTarget (Pressure);
                graphicsDevice.Clear (Color.Black);
                Draw (Temporary);

                DoPressureBoundaries (Pressure);

                graphicsDevice.SetRenderTarget (Temporary);
                graphicsDevice.Clear (Color.Black);
                Draw (Pressure);
            }

            Copy (Temporary, Pressure);

            // Update the shaders copy of the pressure
            graphicsDevice.SetRenderTarget (null);
            Shader.Parameters["Pressure"].SetValue (Pressure);
        }

        //------------------------------------------------------------------
        private void DoSubstract()
        {
            // Subtract the pressure from the velocity
            Shader.CurrentTechnique = Shader.Techniques["Subtract"];
            graphicsDevice.SetRenderTarget (Temporary);
            graphicsDevice.Clear (Color.Black);
            Draw (Velocity);
            Copy (Temporary, Velocity);
        }

        //------------------------------------------------------------------
        private void DoVorticity()
        {
            // Calculate Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticity"];
            graphicsDevice.SetRenderTarget (Vorticity);
            graphicsDevice.Clear (Color.Black);
            Draw (Velocity);

            // Apply Force to Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticityForce"];
            graphicsDevice.SetRenderTarget (Temporary);
            Shader.Parameters["Vorticity"].SetValue (Vorticity); 
            graphicsDevice.Clear (Color.Black);
            Draw (Vorticity);
            Copy (Temporary, Velocity); 
        }

        //-----------------------------------------------------------------
        private void DoVelocityBoundaries ()
        {
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryVelocityBoundary"];

            // Update Velocity Offsets
            graphicsDevice.SetRenderTarget (Temporary);
            graphicsDevice.Clear (Color.Black);
            Draw (Velocity);
            Copy (Temporary, Velocity);
        }

        //-----------------------------------------------------------------
        private void DoPressureBoundaries (RenderTarget2D pressure)
        {
            var backup = Shader.CurrentTechnique;
            Shader.CurrentTechnique = Shader.Techniques["ArbitraryPressureBoundary"];

            // Update Velocity Offsets
            graphicsDevice.SetRenderTarget (TemporaryHelper);
            graphicsDevice.Clear (Color.Black);
            Draw (pressure);
            Copy (TemporaryHelper, pressure);

            Shader.CurrentTechnique = backup;
        }


        //------------------------------------------------------------------
        private void UpdateOffsets()
        {
            Shader.CurrentTechnique = Shader.Techniques["UpdateOffsets"];

            // Update Velocity Offsets
            graphicsDevice.SetRenderTarget (VelocityOffsets);
            Shader.Parameters["OffsetTable"].SetValue (VelocityOffsetsTable);
            graphicsDevice.Clear (Color.Black);
            Draw (Boundaries);

            // Update Pressure Offsets
            graphicsDevice.SetRenderTarget (PressureOffsets);
            Shader.Parameters["OffsetTable"].SetValue (PressureOffsetsTable);
            graphicsDevice.Clear (Color.Black);
            Draw (Boundaries);
        }

        //------------------------------------------------------------------
        private void Render()
        {
            // Smooth and render the fluid to the screen            
            Shader.CurrentTechnique = Shader.Techniques["Final"];
            graphicsDevice.SetRenderTarget (FinalRender);
            graphicsDevice.Clear (Color.Black);
            spriteBatch.Begin (SortMode, Blending, Sampling, null, null, Shader);
            spriteBatch.Draw (Velocity, new Rectangle (0, 0, (int) Params.ScreenSize.X, (int) Params.ScreenSize.Y), Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        private void Copy (RenderTarget2D source, RenderTarget2D destination)
        {
            graphicsDevice.SetRenderTarget (destination);
            graphicsDevice.Clear (Color.Black);

            spriteBatch.Begin (SortMode, Blending, Sampling, null, null);
            spriteBatch.Draw (source, Vector2.Zero, Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        private void Draw (Texture2D texture)
        {
            spriteBatch.Begin (SortMode, Blending, Sampling, null, null, Shader);
            spriteBatch.Draw (texture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        //------------------------------------------------------------------
        public void Draw (Vector2 offset)
        {
            spriteBatch.Draw (FinalRender, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }
    }






    //------------------------------------------------------------------
    public class FluidParams
    {
        // Iterations to perform
        public int Iterations;
        // Size of the fluid grid  
        public int GridSize;
        // Size of the screen the fluid is to be rendered on
        public Vector2 ScreenSize;
        // Delta time, MARKED FOR REMOVAL
        public float dT;
        // Diffusion rate of velocity
        public float VelocityDiffusion;
        // Diffusion rate for density
        public float DensityDiffusion;
        public float VorticityScale;

        public FluidParams()
        {
            Iterations = 20;
            GridSize = 256;
            ScreenSize = new Vector2(1024, 768);
            dT = 1.0f;
            VelocityDiffusion = 1.0f;
            DensityDiffusion = 1.0f;
            VorticityScale = 0.20f;
        }
    }
}
