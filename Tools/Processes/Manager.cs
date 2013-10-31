using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tools.Processes
{
    public class Manager : GameComponent
    {
        private readonly List<Process> processes = new List <Process> ();

        public static Manager Instance;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
        }

        //------------------------------------------------------------------
        public static void Add (Process process)
        {
            Instance.processes.Add (process);
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var process in processes)
            {
                process.Update (elapsed);
            }

            processes.RemoveAll (process => process.Finished);
        }

    }
}