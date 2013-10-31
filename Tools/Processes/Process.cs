using System;
using Android.Graphics.Drawables;
using Microsoft.Xna.Framework;

namespace Tools.Processes
{
    public abstract class Process
    {
        protected float Elapsed;
        protected readonly float Duration;

        public bool Finished { get; set; }

        //------------------------------------------------------------------
        protected Process (float duration)
        {
            Duration = duration;
        }

        //------------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            Elapsed += elapsed;


            if (Elapsed >= Duration)
                Finished = true;
        }

        //-----------------------------------------------------------------
        public void AddToManager ( )
        {
            Manager.Add (this);
        }


    }
}