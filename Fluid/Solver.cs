using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fluid
{
    public class Solver : Unit
    {
        public const int Iterations = 20;
        public const float VelocityDiffusion = 1.0f;
        public const float DensityDiffusion = 0.995f;
        public const float VorticityScale = 0.30f;

        #region Render Targets

        internal RenderTarget2D Velocity;
        internal RenderTarget2D Advected;
        internal RenderTarget2D Reversed;
        internal RenderTarget2D Density;
        internal RenderTarget2D Divergence;
        internal RenderTarget2D Pressure;
        internal RenderTarget2D Vorticity;

        #endregion

        private readonly Obstacles obstacles;
        private readonly Render render;

        private readonly EffectParameter permanentVelocity;

        //------------------------------------------------------------------
        public Data Data { get; private set; }
        public Emitter Emitter { get; private set; }

        //------------------------------------------------------------------
        public Solver (Game game)
            : base (game, "Solver")
        {
            Shader.Parameters["VorticityScale"].SetValue (VorticityScale);
            Shader.Parameters["HalfCellSize"].SetValue (0.5f);

            Velocity = CreateDefaultRenderTarget();
            Advected = CreateDefaultRenderTarget ();
            Reversed = CreateDefaultRenderTarget ();
            Density = CreateDefaultRenderTarget();
            Vorticity = CreateDefaultRenderTarget();
            Pressure = CreateDefaultRenderTarget();
            Divergence = CreateDefaultRenderTarget();

            CreateDefaultRenderTarget();

            Emitter = new Emitter (Game);
//            obstacles = new Obstacles (Game);
            render = new Render (Game);
            Data = new Data (Game);

            permanentVelocity = Shader.Parameters["PermanentVelocity"];
        }

        //------------------------------------------------------------------
        public override void Update()
        {
            obstacles.Update();
            Emitter.AddSplats (Velocity, Density);
            obstacles.ComputeVelocity (Velocity);
            obstacles.ComputeDensity (Density);

//            ComputePermanentAdvection ();
//            obstacles.ComputeVelocity (Velocity);
//            obstacles.ComputeDensity (Density);

            ComputeAdvection ();
            obstacles.ComputeVelocity (Velocity);
            obstacles.ComputeDensity (Density);

            ComputeDivergence ();
            ComputePressure ();
            ComputeSubstract ();
            ComputeVorticity ();

            Data.Process (Velocity);

            Render();

            Debug.Update();
            DebugKeys();
        }

        #region Computations

        //------------------------------------------------------------------
        // Using to simulate Camera moving
        // Actually this Advection just parallely move all particles in a field to the bottom
        private void ComputePermanentAdvection()
        {
            Shader.CurrentTechnique = Shader.Techniques["PermanentAdvection"];

            // Velocity
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            Compute (Velocity);
            Copy (Temporary, Velocity);

            // Density
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            Compute (Density);
            Copy (Temporary, Density);
        }


        //------------------------------------------------------------------
        private void ComputeAdvection()
        {
//            ComputeAdvection (Density, DensityDiffusion);
//            ComputeAdvection (Velocity, VelocityDiffusion);
//            return;





            Shader.Parameters["DT"].SetValue (1.0f);

            // Advect the velocity
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Diffusion"].SetValue (VelocityDiffusion);
            Compute (Velocity);
            Copy (Temporary, Velocity);


            // Advect the density
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            Shader.Parameters["Diffusion"].SetValue (DensityDiffusion);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Compute (Density);
            Copy (Temporary, Density);
        }

        //------------------------------------------------------------------
        private void ComputeAdvection (RenderTarget2D texture, float diffusion)
        {

            // Advected
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            Device.SetRenderTarget (Advected);
            Device.Clear (Color.Black);
            Shader.Parameters["DT"].SetValue (1.0f);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Diffusion"].SetValue (diffusion);
            Compute (texture);

            // Reversed
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            Device.SetRenderTarget (Reversed);
            Device.Clear (Color.Black);
            Shader.Parameters["DT"].SetValue (-1.0f);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Diffusion"].SetValue (diffusion);
            Compute (Advected);

            // MacCormack
            Shader.CurrentTechnique = Shader.Techniques["DoAdvectionMacCormack"];
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            Shader.Parameters["DT"].SetValue (1.0f);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Advected"].SetValue (Advected);
            Shader.Parameters["Reversed"].SetValue (Reversed);
            Shader.Parameters["Diffusion"].SetValue (diffusion);
            Compute (texture);

            Copy (Temporary, texture);
        }

        //------------------------------------------------------------------
        private void ComputeDivergence()
        {
            // Calculate the divergence of the velocity
            Shader.CurrentTechnique = Shader.Techniques["DoDivergence"];
            Device.SetRenderTarget (Divergence);
            Device.Clear (Color.Black);
            Compute (Velocity);

            // Update the shaders copy of the divergence
            Device.SetRenderTarget (null);
            Shader.Parameters["Divergence"].SetValue (Divergence);
        }

        //------------------------------------------------------------------
        private void ComputePressure()
        {
            // Iterate over the grid calculating the pressure
            Shader.CurrentTechnique = Shader.Techniques["DoJacobi"];

            // Clear Temp
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);

            for (int i = 0; i < Iterations; i++)
            {
                Device.SetRenderTarget (Pressure);
                Device.Clear (Color.Black);
                Compute (Temporary);

                obstacles.ComputePressure (Pressure);

                Device.SetRenderTarget (Temporary);
                Device.Clear (Color.Black);
                Compute (Pressure);
            }

            Copy (Temporary, Pressure);

            // Update the shaders copy of the pressure
            Device.SetRenderTarget (null);
            Shader.Parameters["Pressure"].SetValue (Pressure);
        }

        //------------------------------------------------------------------
        private void ComputeSubstract()
        {
            // Subtract the pressure from the velocity
            Shader.CurrentTechnique = Shader.Techniques["Subtract"];
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            Compute (Velocity);
            Copy (Temporary, Velocity);
        }

        //------------------------------------------------------------------
        private void ComputeVorticity()
        {
            // Calculate Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticity"];
            Device.SetRenderTarget (Vorticity);
            Device.Clear (Color.Black);
            Compute (Velocity);

            // Apply Force to Vorticity
            Shader.CurrentTechnique = Shader.Techniques["DoVorticityForce"];
            Device.SetRenderTarget (Temporary);
            Shader.Parameters["Vorticity"].SetValue (Vorticity);
            Device.Clear (Color.Black);
            Compute (Vorticity);
            Copy (Temporary, Velocity);
        }

        #endregion

        //------------------------------------------------------------------
        private void Render()
        {
            render.DrawInterpolated (Density);
//            render.DrawField (Velocity);
//            render.DrawGradient (Velocity, Density);
        }

        //-----------------------------------------------------------------
        public void Draw()
        {
            render.DrawOnScreen();
        }

        //-----------------------------------------------------------------
        public void SetScene (RenderTarget2D scene)
        {
            obstacles.Scene = scene;
        }

        //------------------------------------------------------------------
        public void SetSpeed (float velocity)
        {
            permanentVelocity.SetValue (velocity * 0.005f);
        }

        private void DebugKeys()
        {
//            if (Keyboard.GetState().IsKeyDown (Keys.A))
//                Variable -= step;
//                            
//            if (Keyboard.GetState ().IsKeyDown (Keys.D))
//                Variable += step;
        }
    }
}