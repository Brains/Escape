using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Engine.Tools.Markers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Point = Microsoft.Xna.Framework.Point;

namespace Fluid
{
    public class Data : Unit
    {
        private new const int Size = 32;

        private readonly RenderTarget2D input;
        private readonly HalfVector4[] data;

        //------------------------------------------------------------------
        public Data (Game game) : base (game, "Data")
        {
            input = new RenderTarget2D (Device, Size, Size, false, Surface, ZFormat);
            data = new HalfVector4[Size * Size];
        }

        //-----------------------------------------------------------------
        public override void Update()
        {
            throw new NotImplementedException();
        }

        //-----------------------------------------------------------------
        public void Process (RenderTarget2D texture)
        {
            Copy (texture);
            input.GetData (data);

//            Draw();
           // DrawStreamline();
//            Debug();
        }

        //------------------------------------------------------------------
        private void Copy (RenderTarget2D source)
        {
            Shader.CurrentTechnique = Shader.Techniques["Interpolation"];
            Device.SetRenderTarget (input);
            Device.Clear (Color.Black);

            Batch.Begin (Sorting, Blending, SamplerState.AnisotropicClamp, null, null, Shader);

            var scale = new Vector2 ((float) Size / Unit.Size, (float) Size / Unit.Size);
            Batch.Draw (source, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Batch.End();

            Device.SetRenderTarget (null);
        }

        //------------------------------------------------------------------
        public Vector3 GetData (Vector2 screen)
        {
            Vector4 value1, value2, value3, value4;

            Point position = GetTextureCoordinates (screen);
            var interpolated = GetInterpolatedData (position, out value1, out value2, out value3, out value4);
            var torque = CalculateTorque (value1, value2, value3, value4);

            return new Vector3 (interpolated.X, interpolated.Y, torque);
        }

        //------------------------------------------------------------------
        private Vector4 GetInterpolatedData (Point position, out Vector4 value1, out Vector4 value2, out Vector4 value3, out Vector4 value4)
        {
            const int offset = 2;
            value1 = data[GetIndex (position.X + offset, position.Y)].ToVector4();
            value2 = data[GetIndex (position.X - offset, position.Y)].ToVector4();
            value3 = data[GetIndex (position.X, position.Y + offset)].ToVector4();
            value4 = data[GetIndex (position.X, position.Y - offset)].ToVector4();

            var lerp1 = Vector4.Lerp (value1, value2, 0.5f);
            var lerp2 = Vector4.Lerp (value3, value4, 0.5f);
            var interpolated = Vector4.Lerp (lerp1, lerp2, 0.5f);
            return interpolated;
        }

        //------------------------------------------------------------------
        private float CalculateTorque (Vector4 value1, Vector4 value2, Vector4 value3, Vector4 value4)
        {
            const float ratio = (float) 800 / 480;
            float torque1 = value1.Y * 1; // 1 is the moment arm
            float torque2 = value2.Y * 1;
            float torque3 = value3.X * ratio; // Fluid is square but the screen isn't. So moment arm on its horizont is higher
            float torque4 = value4.X * ratio;

            float torque = torque1 + torque2 + torque3 + torque4;
            return torque;
        }

        //------------------------------------------------------------------
        private int GetIndex (int x, int y)
        {
            x = MathHelper.Clamp (x, 0, Size - 1);
            y = MathHelper.Clamp (y, 0, Size - 1);

            return y * Size + x;
        }

        //------------------------------------------------------------------
        private Point GetTextureCoordinates (Vector2 screen)
        {
            Vector2 screenSize = new Vector2 (Device.Viewport.Width, Device.Viewport.Height);

            var textureCoordinatesFactor = new Vector2 (Size / screenSize.X, Size / screenSize.Y);
            Vector2 position = screen * textureCoordinatesFactor;

            position = new Vector2 ((float) Math.Round (position.X), (float) Math.Round (position.Y));

            return new Point ((int) position.X, (int) position.Y);
        }

        //------------------------------------------------------------------
        private void Draw()
        {
            Device.SetRenderTarget (null);
//            Device.Clear (Color.Orange);
            Batch.Begin (SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);

            Vector2 scale = new Vector2 ((float) 480 / Size, (float) 800 / Size);
            Batch.Draw (input, new Vector2 (0, 0), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            scale = new Vector2 ((float) 480 / 256, (float) 800 / 256);
//            Batch.Draw (Input, new Vector2 (0, 0), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Batch.End();
        }



        #region Debug

        //------------------------------------------------------------------
        public void Debug ()
        {
            List <HalfVector4> list = data.ToList();
            float min = list.Max (vector4 => vector4.ToVector4().X);

            Batch.Begin();

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    DrawCell (i, j);
                }
            }

            Batch.End();
        }

        //------------------------------------------------------------------
        private void DrawCell (int i, int j)
        {
            var value = data[i * Size + j].ToVector4();

            Vector2 scale = new Vector2 ((float) 480 / Size, (float) 800 / Size);
            Vector2 position = new Vector2 (j, i) * scale;
            Vector2 increment = new Vector2 (value.X, value.Y) * 10;

            if (increment.Length() > 1)
                new Line (position, position + increment, Color.DarkOrange);


            //if (value.Length() > 0.2f)
            //{
            //    Vector2 screen = new Vector2 (j * 480.0f / Size, i * 800.0f / Size);
            //    Vector2 velocity = new Vector2 (value.X, value.Y);

            //    new Line (screen, screen + velocity * 30, Color.DarkOrange);
            //}
        }

        //------------------------------------------------------------------
        private void DrawStreamline ()
        {
            Vector2 scale = new Vector2 ((float) 480 / Size, (float) 800 / Size);
            var screen = Vector2.Zero;

            //            foreach (var x in Enumerable.Range (0, 32))

            Point position = GetTextureCoordinates (screen);
            Vector4 value = data[GetIndex (position.X, position.Y)].ToVector4 ();

            var streamline = new NumericalMethods.Simpson (x => x * value.Y / value.X, 0, 10, 1);

            //            foreach (var point in Enumerable.Range (0, 32))
            //            {
            //                Point position = GetTextureCoordinates (screen);
            //                Vector4 value = data[GetIndex (position.X, position.Y)].ToVector4();
            //                Vector2 increment = new Vector2 (value.X, value.Y * value.X) ;
            //                new Line (screen, screen + increment, Color.DarkOrange);
            //
            //                screen += increment;
            //            }
        }

        #endregion
    }
}