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
        public const float DT = 1.0f;
        public const float VelocityDiffusion = 1.0f;
        public const float DensityDiffusion = 0.995f;
        public const float VorticityScale = 0.30f;

        #region Render Targets
        internal RenderTarget2D Velocity;
        internal RenderTarget2D Density;
        internal RenderTarget2D Divergence;
        internal RenderTarget2D Pressure;
        internal RenderTarget2D Vorticity;
        
        private readonly RenderTarget2D helper;
        #endregion

        private readonly Obstacles obstacles;
        private readonly Render render;

        //------------------------------------------------------------------
        public Data Data { get; private set; }
        public Emitter Emitter { get; private set; }

        //------------------------------------------------------------------
        public Solver (Game game)
            : base (game, "Solver")
        {
            Shader.Parameters["VelocityDiffusion"].SetValue (VelocityDiffusion);
            Shader.Parameters["DensityDiffusion"].SetValue (DensityDiffusion);
            Shader.Parameters["VorticityScale"].SetValue (VorticityScale);
            Shader.Parameters["DT"].SetValue (DT);
            Shader.Parameters["HalfCellSize"].SetValue (0.5f);

            Velocity = CreateDefaultRenderTarget();
            Density = CreateDefaultRenderTarget();
            Vorticity = CreateDefaultRenderTarget();
            Pressure = CreateDefaultRenderTarget();
            Divergence = CreateDefaultRenderTarget();

            helper = CreateDefaultRenderTarget();

            Emitter = new Emitter(Game);
            obstacles = new Obstacles(Game);
            render = new Render(Game);
            Data = new Data(Game);
        }

        //------------------------------------------------------------------

        public override void Update()
        {
            obstacles.Update();
            Emitter.AddSplats (Velocity, Density);
            obstacles.ComputeVelocity (Velocity);
            obstacles.ComputeDensity (Density);

            ComputeAdvect ();
            obstacles.ComputeVelocity (Velocity);
            ComputeDivergence ();
            ComputePressure ();
            ComputeSubstract ();
            ComputeVorticity ();

            Data.Process(Velocity);

            Debug.Update();
        }

        #region Computations

        //------------------------------------------------------------------
        private void ComputeAdvect ()
        {
            // Advect the velocity and density
            // Other quantities can be advected too
            Shader.CurrentTechnique = Shader.Techniques["DoAdvection"];
            Device.SetRenderTargets (Temporary, helper);
            Device.Clear (Color.Black);
            Shader.Parameters["Velocity"].SetValue (Velocity);
            Shader.Parameters["Density"].SetValue (Density);

            Compute (Velocity);
            Copy (Temporary, Velocity);
            Copy (helper, Density);
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

        //-----------------------------------------------------------------
        public void Render ()
        {
            render.DrawInterpolated (Density);
//            render.DrawField (Velocity);
//            render.DrawGradient (Pressure, Density);
            render.DrawOnScreen ();
        }

        //-----------------------------------------------------------------
        public void SetScene (RenderTarget2D scene)
        {
            obstacles.Scene = scene;
        }
    }
}