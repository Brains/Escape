#region File Description

//-----------------------------------------------------------------------------
// AnimatedTexture.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Traffic.Cars;

namespace Animation
{
    internal class AnimatedTexture : Traffic.Object
    {
        const int Width = 64;
        const int Height = 64;

        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int Frame;
        private float TotalElapsed;
        private bool Paused;

        public float Rotation, Scale, Depth;
        public Vector2 Origin;

        //------------------------------------------------------------------
        public AnimatedTexture (Car car, float scale, float depth) : base (car)
        {
            this.Scale = scale;
            this.Depth = depth;
        }

        //------------------------------------------------------------------
        public void Load (Texture2D texture,
            int frameCount, int framesPerSec)
        {
            framecount = frameCount;
            myTexture = texture;
            TimePerFrame = (float) 1 / framesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;

            Origin = new Vector2 (Width / 2, Height / 2);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);

            UpdateFrame (elapsed);
        }

        //------------------------------------------------------------------
        public void UpdateFrame (float elapsed)
        {
            if (Paused)
                return;
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                Frame++;
                // Keep the Frame between 0 and the total frames, minus one.
                Frame = Frame % framecount;
                TotalElapsed -= TimePerFrame;
            }
        }

        //------------------------------------------------------------------
        public override void Draw (SpriteBatch spriteBatch)
        {
            base.Draw (spriteBatch);

            DrawFrame (spriteBatch, GlobalPosition);
        }

        //------------------------------------------------------------------
        public void DrawFrame (SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame (batch, Frame, screenPos);
        }

        //------------------------------------------------------------------
        public void DrawFrame (SpriteBatch batch, int frame, Vector2 screenPos)
        {
            int x = Width * (frame % 8);
            int y = Height * (frame / 8);
            Rectangle sourcerect = new Rectangle (x, y, Width, Height);

            batch.Draw (myTexture, GlobalPosition, sourcerect, Color.White, Rotation, Origin, Scale, SpriteEffects.None, Depth);
        }

        //------------------------------------------------------------------
        public bool IsPaused
        {
            get { return Paused; }
        }

        //------------------------------------------------------------------
        public void Reset ()
        {
            Frame = 0;
            TotalElapsed = 0f;
        }

        //------------------------------------------------------------------
        public void Stop ()
        {
            Pause ();
            Reset ();
        }

        //------------------------------------------------------------------
        public void Play ()
        {
            Paused = false;
        }

        //------------------------------------------------------------------
        public void Pause ()
        {
            Paused = true;
        }
    }
}