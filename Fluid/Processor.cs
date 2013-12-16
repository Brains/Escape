using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Fluid
{
    public class Processor
    {
        private Simulation simulation;
        private GraphicsDevice graphicsDevice;
        private RenderTarget2D Output;
        private int size;
        private SpriteFont font;
        private Texture2D brush;

        //------------------------------------------------------------------
        public Processor (Simulation simulation, GraphicsDevice graphicsDevice, ContentManager Content)
        {
            this.simulation = simulation;
            this.graphicsDevice = graphicsDevice;

            size = 64;
            Output = new RenderTarget2D (graphicsDevice, size, size, false, SurfaceFormat.HdrBlendable, DepthFormat.None);

            font = Content.Load <SpriteFont> ("Debug");
            brush = Content.Load <Texture2D> ("brush");
        }

        //-----------------------------------------------------------------
        public void Analyze (SpriteBatch batch)
        {
            Copy (batch);

            HalfVector4[] data = new HalfVector4[size * size];
            Output.GetData (data);
            List <HalfVector4> list = data.ToList();


            float min = list.Max (vector4 => vector4.ToVector4().X);
            batch.Begin();
            batch.DrawString (font, min.ToString ("F3"), new Vector2 (50), Color.Maroon);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float value = data[i * size + j].ToVector4 ().X;

                    if (value > 1.0f)
                        batch.Draw (brush, new Vector2 (i, j) * 10, null, Color.White, 0.0f, Vector2.Zero, value / 20, SpriteEffects.None, 0.0f);
                }
            }

            batch.End ();

        }

        //------------------------------------------------------------------
        private void Copy (SpriteBatch batch)
        {
            graphicsDevice.SetRenderTarget (Output);
            graphicsDevice.Clear (Color.Black);

            throw new NotImplementedException(); 
//            batch.Begin (simulation.SortMode, simulation.Blending, simulation.Sampling, null, null, simulation.Shader);
//            var rectangle = new Rectangle (0, 0, 64 / 256, 64 / 256);
//            batch.Draw (simulation.Pressure, Vector2.Zero, rectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
//            batch.End();

            graphicsDevice.SetRenderTarget (null);
            graphicsDevice.Clear (Color.Orange);
//            batch.Begin (SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
//            batch.Draw (Output, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 10, SpriteEffects.None, 0);
//            batch.End();
        }
    }
}