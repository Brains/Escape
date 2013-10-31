using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tools.Timers
{
    public class Manager : GameComponent
    {
        private List <Timer> Timers = new List <Timer> ();
        private List <Timer> ToRemove = new List <Timer> ();

        public static Manager Instance;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
        }

        //------------------------------------------------------------------
        public static void Add (Timer Timer)
        {
            Instance.Timers.Add (Timer);
        }

        //------------------------------------------------------------------
        public static void Remove (Timer Timer)
        {
            Instance.ToRemove.Add (Timer);
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gametime)
        {
            foreach (var timer in Timers) 
                timer.Update ((float) gametime.ElapsedGameTime.TotalSeconds);

            foreach (var timer in ToRemove)
                Timers.Remove (timer);

            ToRemove.Clear ();
        }
    }
}