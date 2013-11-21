using System;

namespace Tools.Timers
{
    public class Loop : Timer
    {
        //------------------------------------------------------------------
        public override void Update (float seconds)
        {
            Elapsed += seconds;

            if (Elapsed >= Interval)
            {
                Trigger.Invoke ();
                Elapsed = 0;
            }
        }

        //------------------------------------------------------------------
        public new static void Create (float interval, Action trigger)
        {
            Loop timer = new Loop { Interval = interval, Trigger = trigger };

            Manager.Add (timer);
        }
    }
}