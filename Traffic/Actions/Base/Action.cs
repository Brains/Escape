namespace Traffic.Actions.Base
{
    public abstract class Action
    {
        protected float Elapsed;
        protected readonly float Duration;

        public bool Finished { get; set; }
//        public bool Lock { get; set; }
        public string Name { get; set; }

//        public event Action Finish = delegate { };

        //------------------------------------------------------------------
        protected Action (float duration)
        {
            Duration = duration;
            Name = GetType ().Name;
        }

        //------------------------------------------------------------------
        protected Action ()
        {
            Duration = 0;
            Name = GetType ().Name;
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

        //------------------------------------------------------------------
        public override string ToString ()
        {
            return Name;
        }
    }
}