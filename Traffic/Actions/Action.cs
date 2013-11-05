namespace Traffic.Actions
{
    public abstract class Action
    {
        protected float Elapsed;
        protected readonly float Duration;

        public bool Finished { get; set; }
        public bool Lock { get; set; }

//        public event Action Finish = delegate { };

        //------------------------------------------------------------------
        protected Action (float duration)
        {
            Duration = duration;
        }

        //------------------------------------------------------------------
        protected Action ()
        {
            Duration = 0;
        }

        //------------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            Elapsed += elapsed;

            if (Elapsed >= Duration)
            {
//                Finish ();
                Finished = true;
            }
        }

        //-----------------------------------------------------------------
        public void AddToManager ()
        {
            Manager.Add (this);
        }
    }
}