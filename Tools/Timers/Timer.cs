using System;

namespace Tools.Timers
{
    public class Timer
    {
        public Action Trigger;
        public float Interval;
        private float elapsed;

        //------------------------------------------------------------------
        public Timer () {}

        //------------------------------------------------------------------
        public void Update (float seconds)
        {
            elapsed += seconds;

            if (elapsed >= Interval)
            {
                Trigger.Invoke ();
                Destroy ();
            }
        }

        //------------------------------------------------------------------
        public void Destroy ()
        {
            Manager.Remove (this);
        }

        //------------------------------------------------------------------
        public static void Create (float interval, Action trigger)
        {
            Timer timer = new Timer () {Interval = interval, Trigger = trigger};

            Manager.Add (timer);
        }
    }
}