using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fluid
{
    public class Emitter : Unit
    {
        public List <KeyValuePair <Vector2, Vector2>> Impulses { get; set; }
        public List <KeyValuePair <Texture2D, Vector2>> Particles { get; set; }

        internal RenderTarget2D NewVelocities;
        internal RenderTarget2D NewDensities;

        private readonly Texture2D brush;
        private readonly RenderTarget2D helper;

        private readonly EffectParameter velocity;
        private readonly EffectParameter density;
        private readonly EffectParameter newVelocities;
        private readonly EffectParameter newDensities;
        private readonly EffectParameter impulse;
        private readonly EffectParameter fraction;
        
        private readonly BlendState blend = BlendState.AlphaBlend;

        //------------------------------------------------------------------
        public Color Color { get; set; }

        //------------------------------------------------------------------
        public Emitter (Game game) : base (game, "Emitter")
        {
            Color = Color.White;

            Impulses = new List<KeyValuePair<Vector2, Vector2>> ();
            Particles = new List <KeyValuePair <Texture2D, Vector2>>();

            Viewport viewport = Device.Viewport;
            NewVelocities = new RenderTarget2D (Device, viewport.Width, viewport.Height, false, Surface, ZFormat);
            NewDensities = new RenderTarget2D (Device, viewport.Width, viewport.Height, false, Surface, ZFormat);

            helper = CreateDefaultRenderTarget();
            brush = Game.Content.Load <Texture2D> ("Fluid/Brush");

            velocity = Shader.Parameters["Velocity"];
            density = Shader.Parameters["Density"];
            newVelocities = Shader.Parameters["NewVelocities"];
            newDensities = Shader.Parameters["NewDensities"];
            impulse = Shader.Parameters["Impulse"];
            fraction = Shader.Parameters["Fraction"];
        }

        //------------------------------------------------------------------
        public override void Update()
        {
            throw new NotImplementedException(); 
        }

        //------------------------------------------------------------------
        internal void AddSplats (RenderTarget2D Velocity, RenderTarget2D Density)
        {
            AddVelocity ();
            AddDensity ();

            // Add the velocity and density splats into the fluid
            Shader.CurrentTechnique = Shader.Techniques["DoAddSources"];
            Device.SetRenderTargets (Temporary, helper);
            Device.Clear (Color.Black);

            velocity.SetValue (Velocity);
            density.SetValue (Density);
            newVelocities.SetValue (NewVelocities);
            newDensities.SetValue (NewDensities);

            Compute (Velocity);
            Copy (Temporary, Velocity);
            Copy (helper, Density);
        }

        //------------------------------------------------------------------
        private void AddVelocity()
        {
            Shader.CurrentTechnique = Shader.Techniques["VelocityColorize"];
            Device.SetRenderTarget (NewVelocities);
            Device.Clear (Color.Black);

//            AddGeneralFlow();
            AddImpulses();
        }

        //------------------------------------------------------------------
        private void AddGeneralFlow()
        {
            var scale = new Vector2 (8, 1);
            impulse.SetValue (new Vector4 (0, -0.005f, 0, 0));

            Batch.Begin (Sorting, blend, Sampling, null, null, Shader);
            Batch.Draw (brush, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            Batch.Draw (brush, new Vector2 (0, 400), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            Batch.End();
        }

        //------------------------------------------------------------------
        private void AddImpulses()
        {
            if (Debug.LeftMouse ()) AddImpulse (Debug.Direction(), Debug.Mouse());

            foreach (var pair in Impulses)
            {
                // Must Invert, don't know why
                var force = pair.Key * -1;
                var position = pair.Value;

                impulse.SetValue (new Vector4 (force.X, force.Y, 0, 0));

                Batch.Begin (Sorting, blend, Sampling, null, null, Shader);
                Batch.Draw (brush, position, null, Color.White, 0.0f, new Vector2 (32), 0.3f, SpriteEffects.None, 0.0f);
                Batch.End();
            }

            Impulses.Clear();
        }

        //------------------------------------------------------------------
        public void AddImpulse (Vector2 impulse, Vector2 position)
        {
            Impulses.Add (new KeyValuePair <Vector2, Vector2> (impulse, position));
        }

        //------------------------------------------------------------------
        public void AddDensity()
        {
            Device.SetRenderTarget (NewDensities);
            Device.Clear (Color.Black);

//            AddGeneralDensity();
            AddParticles();
        }

        //------------------------------------------------------------------
        private void AddGeneralDensity()
        {
            fraction.SetValue (16.0f);
            var scale = new Vector2 (8, 1);

            Batch.Begin (Sorting, blend, Sampling, null, null);
            Batch.Draw (brush, Vector2.Zero, null, Color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            Batch.End();
        }

        //------------------------------------------------------------------
        private void AddParticles ()
        {
            fraction.SetValue (1.0f);

            if (Debug.LeftMouse ()) AddParticle (brush, Debug.Mouse ());

            foreach (var particle in Particles)
            {
                Batch.Begin (Sorting, blend, Sampling, null, null);
                Batch.Draw (particle.Key, particle.Value, null, Color.Orange, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.0f);
                Batch.End();
            }

            Particles.Clear ();
        }

        //------------------------------------------------------------------
        public void AddParticle (Texture2D texture, Vector2 position)
        {
            Particles.Add (new KeyValuePair <Texture2D, Vector2> (texture, position));
        }
    }
}