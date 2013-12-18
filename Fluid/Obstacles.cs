using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    public class Obstacles : Unit
    {
        internal Texture2D VelocityTable;
        internal Texture2D PressureTable;

        internal RenderTarget2D Boundaries;
        internal RenderTarget2D VelocityOffsets;
        internal RenderTarget2D PressureOffsets;

        private readonly EffectParameter table;
        private RenderTarget2D helper;

        //------------------------------------------------------------------
        public RenderTarget2D Scene { get; set; }

        //------------------------------------------------------------------
        public Obstacles (Game game)
            : base (game, "Obstacles")
        {
            Boundaries = CreateDefaultRenderTarget();
            VelocityOffsets = CreateDefaultRenderTarget();
            PressureOffsets = CreateDefaultRenderTarget();

            Shader.Parameters["Boundaries"].SetValue (Boundaries);
            Shader.Parameters["VelocityOffsets"].SetValue (VelocityOffsets);
            Shader.Parameters["PressureOffsets"].SetValue (PressureOffsets);

            table = Shader.Parameters["OffsetTable"];
            helper = CreateDefaultRenderTarget ();

            CreateVelocityTable ();
            CreatePressureTable ();
        }

        //------------------------------------------------------------------
        private void CreateVelocityTable()
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

            VelocityTable = new Texture2D (Device, 34, 1, false, SurfaceFormat.Vector4);
            VelocityTable.SetData (data);
        }

        //------------------------------------------------------------------
        private void CreatePressureTable ()
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

            PressureTable = new Texture2D (Device, 34, 1, false, SurfaceFormat.Vector4);
            PressureTable.SetData (data);
        }

        //------------------------------------------------------------------
        public override void Update()
        {
            ShapeObstacles ();
            UpdateOffsets();
        }

        //------------------------------------------------------------------
        private void ShapeObstacles()
        {
            Vector2 scale = new Vector2 ((float) Size / Scene.Width, (float) Size / Scene.Height);

            Shader.CurrentTechnique = Shader.Techniques["ShapeObstacles"];
            Device.SetRenderTarget (Boundaries);
            Device.Clear (Color.Black);

            Batch.Begin (Sorting, Blending, Sampling, null, null, Shader);
            Batch.Draw (Scene, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            Batch.End();

            Device.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        internal void UpdateOffsets ()
        {
            Shader.CurrentTechnique = Shader.Techniques["UpdateOffsets"];

            // Update VelocityOffsets Offsets
            Device.SetRenderTarget (VelocityOffsets);
            Device.Clear (Color.Black);
            table.SetValue (VelocityTable);
            Compute (Boundaries);

            // Update PressureOffsets Offsets
            Device.SetRenderTarget (PressureOffsets);
            Device.Clear (Color.Black);
            table.SetValue (PressureTable);
            Compute (Boundaries);
        }

        //------------------------------------------------------------------
        internal void ComputeVelocity (RenderTarget2D velocity)
        {
            Shader.CurrentTechnique = Shader.Techniques["Velocity"];
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);
            
            Compute (velocity);
            Copy (Temporary, velocity);
        }

        //------------------------------------------------------------------
        public void ComputeDensity (RenderTarget2D density)
        {
            Shader.CurrentTechnique = Shader.Techniques["Density"];
            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);

            Compute (density);
            Copy (Temporary, density);
        }

        //------------------------------------------------------------------
        public void ComputePressure (RenderTarget2D pressure)
        {
            var backup = Shader.CurrentTechnique;
            Shader.CurrentTechnique = Shader.Techniques["Pressure"];

            Device.SetRenderTarget (Temporary);
            Device.Clear (Color.Black);

            Compute (pressure);
            Copy (Temporary, pressure);

            Shader.CurrentTechnique = backup;
        }
    }
}