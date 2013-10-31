using System;

namespace Tools.Processes
{
    public class Generic : Process
    {
        private readonly Action action;

        //------------------------------------------------------------------
        public Generic (Action action) : base (0)
        {
            this.action = action;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            action.Invoke ();

            Finished = true;
        }

    }
}