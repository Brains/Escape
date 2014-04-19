using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    public class Render : Unit
    {
        internal Texture2D Gradient;

        //------------------------------------------------------------------
        public Render (Game game) : base (game, "Render")
        {
            Gradient = game.Content.Load <Texture2D> ("Fluid/Gradient");
//            Shader.Parameters["Map"].SetValue (Gradient);

            Viewport viewport = Device.Viewport;
            Output = CreateDefaultRenderTarget();
//            Output = new RenderTarget2D (Device, viewport.Width, viewport.Height, false, Surface, ZFormat);
;
            Gradient = Game.Content.Load <Texture2D> ("Fluid/Gradient");
        }

        //------------------------------------------------------------------
        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        //------------------------------------------------------------------
        public RenderTarget2D DrawGradient (Texture2D texture, Texture2D stencil)
        {
            Shader.CurrentTechnique = Shader.Techniques["Gradient"];
            Shader.Parameters["Stencil"].SetValue (stencil);
            Device.SetRenderTarget (Output);
            Device.Clear (Color.Black);
            Compute (texture);

            Device.SetRenderTarget (null);

            return Output;
        }

        //------------------------------------------------------------------
        public RenderTarget2D DrawInterpolated (Texture2D texture)
        {
            Shader.CurrentTechnique = Shader.Techniques["Interpolation"];
            Device.SetRenderTarget (Output);
            Device.Clear (Color.Black);
            Compute (texture);

            Device.SetRenderTarget (null);

            return Output;
        }

        //------------------------------------------------------------------
        public RenderTarget2D DrawField (Texture2D texture)
        {
            Shader.CurrentTechnique = Shader.Techniques["Display"];
            Device.SetRenderTarget (Output);
            Device.Clear (Color.Black);
            Compute (texture);

            Device.SetRenderTarget (null);

            return Output;
        }

        //------------------------------------------------------------------
        public void DrawOnScreen()
        {
            var rectangle = new Rectangle (0, 0, Device.Viewport.Width, Device.Viewport.Height);
            var scale = new Vector2 ((float) Device.Viewport.Width / Size, (float) Device.Viewport.Height / Size);

            Device.SetRenderTarget (null);
//            Device.Clear (Color.White);

            Batch.Begin (SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, null, null);
            Batch.Draw (Output, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale , SpriteEffects.None, 1.0f);
            Batch.End();
        }
    }
}