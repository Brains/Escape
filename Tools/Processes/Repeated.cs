using System;

namespace Tools.Processes
{
    public class Repeated : Process
    {
        private readonly Action action;
        private int counter;
        private int times;

        //------------------------------------------------------------------
        public Repeated (Action action, int times) : base (0)
        {
            this.action = action;
            this.times = times;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            action.Invoke ();
            counter++;

            if (counter >= times)
                Finished = true;
        }

    }
}