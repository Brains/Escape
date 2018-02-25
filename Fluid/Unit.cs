using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    abstract public class Unit 
    {
        //------------------------------------------------------------------
        protected const int Size = 256;

        //------------------------------------------------------------------
        protected Game Game;
        protected Effect Shader;
        protected SpriteBatch Batch;
        protected GraphicsDevice Device;
        protected RenderTarget2D Output;

        //------------------------------------------------------------------
        public readonly BlendState Blending = BlendState.Opaque;
        public readonly SamplerState Sampling = new SamplerState();
        internal RenderTarget2D Temporary;
        public const SpriteSortMode Sorting = SpriteSortMode.Immediate;
        public const SurfaceFormat Surface = SurfaceFormat.HalfVector4;
        public const DepthFormat ZFormat = DepthFormat.None;

        //------------------------------------------------------------------
        protected Unit (Game game, string shader)
        {
            Game = game;
            Device = Game.GraphicsDevice;
            Batch = new SpriteBatch (Device);
            
            Output = CreateDefaultRenderTarget();
            Temporary = CreateDefaultRenderTarget();

            SetupSemplaerStates ();

            // Shader
            if (shader == null) return;
            Shader = Game.Content.Load<Effect> ("Fluid/" + shader);
            Shader.Parameters["Size"].SetValue ((float) Size);
            Shader.Parameters["Shift"].SetValue (new Vector2 (0.5f / Size, 0.5f / Size));
        }

        //------------------------------------------------------------------
        protected void SetupSemplaerStates()
        {
            Sampling.AddressU = TextureAddressMode.Clamp;
            Sampling.AddressV = TextureAddressMode.Clamp;
            Sampling.AddressW = TextureAddressMode.Clamp;
            Sampling.Filter = TextureFilter.Point;
            for (int i = 0; i < 16; i++)
                Device.SamplerStates[i] = Sampling;
        }

        //------------------------------------------------------------------
        protected RenderTarget2D CreateDefaultRenderTarget()
        {
            return new RenderTarget2D (Device, Size, Size, false, Surface, ZFormat);
        }

        //-----------------------------------------------------------------
        abstract public void Update ();

        //-----------------------------------------------------------------
        protected void Copy (RenderTarget2D source, RenderTarget2D destination)
        {
            Device.SetRenderTarget (destination);
            Device.Clear (Color.Black);

            Batch.Begin (Sorting, Blending, Sampling, null, null);
            Batch.Draw (source, Vector2.Zero, Color.White);
            Batch.End();

            Device.SetRenderTarget (null);
        }

        //-----------------------------------------------------------------
        protected void Compute (Texture2D texture)
        {
            Batch.Begin (Sorting, Blending, Sampling, null, null, Shader);
            Batch.Draw (texture, Vector2.Zero, Color.White);
            Batch.End();
        }
    }
}