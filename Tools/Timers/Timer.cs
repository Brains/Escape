using System;

namespace Tools.Timers
{
    public class Timer
    {
        public Action Trigger;
        public float Interval;
        protected float Elapsed;

        public event Action Finish  = delegate { };

        //------------------------------------------------------------------
        public Timer () {}

        //------------------------------------------------------------------
        public virtual void Update (float seconds)
        {
            Elapsed += seconds;

            if (Elapsed >= Interval)
            {
                Trigger.Invoke ();
                Destroy ();
            }
        }

        //------------------------------------------------------------------
        public void Destroy ()
        {
            Finish();
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