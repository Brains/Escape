using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Tools.Processes
{
    public class Sequence : Process
    {
        private readonly List <Process> processes = new List <Process> ();

        //------------------------------------------------------------------
        public Sequence () : base (0) {}

        //------------------------------------------------------------------
        public void Add (Process process)
        {
            processes.Add (process);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            if (processes.Count <= 0) return;

            processes.First ().Update (elapsed);

            if (processes.First ().Finished)
                processes.Remove (processes.First ());
        }
    }
}