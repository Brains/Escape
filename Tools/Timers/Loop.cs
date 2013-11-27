using System;

namespace Tools.Timers
{
    public class Loop : Timer
    {
        public int IterationsLimit;

        private int counter;

        //------------------------------------------------------------------
        public override void Update (float seconds)
        {
            Elapsed += seconds;

            if (Elapsed >= Interval)
            {
                Trigger.Invoke ();
                Elapsed = 0;
                counter++;
            }

            // Infinite iterations
            if (IterationsLimit == 0) return;

            // Finite iterations
            if (counter > IterationsLimit) 
                Destroy ();
        }

        //------------------------------------------------------------------
        public static void Create (float interval, int iterationsLimit, Action trigger)
        {
            Loop timer = new Loop {Interval = interval, IterationsLimit = iterationsLimit, Trigger = trigger};

            Manager.Add (timer);
        }
    }
}