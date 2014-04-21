using System;

namespace Engine.Tools.Timers
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
            if (counter >= IterationsLimit) 
                Destroy ();
        }

        //------------------------------------------------------------------
        public static void Create (float interval, int iterationsLimit, Action trigger, Action finish = null)
        {
            Loop timer = new Loop {Interval = interval, IterationsLimit = iterationsLimit, Trigger = trigger};
            timer.Finish += finish;

            Manager.Add (timer);
        }
    }
}