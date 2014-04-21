using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Tools.Timers
{
    public class Manager : GameComponent
    {
        private readonly List <Timer> timers = new List <Timer> ();
        private readonly List <Timer> toRemove = new List <Timer> ();

        public static Manager Instance;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
        }

        //------------------------------------------------------------------
        public static void Add (Timer timer)
        {
            Instance.timers.Add (timer);
        }

        //------------------------------------------------------------------
        public static void Remove (Timer timer)
        {
            Instance.toRemove.Add (timer);
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gametime)
        {
            foreach (var timer in timers) 
                timer.Update ((float) gametime.ElapsedGameTime.TotalSeconds);

            foreach (var timer in toRemove)
                timers.Remove (timer);

            toRemove.Clear ();
        }
    }
}