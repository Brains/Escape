using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Traffic.Actions
{
    public class Manager : GameComponent
    {
        private readonly List<Action> processes = new List <Action> ();

        public static Manager Instance;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
        }

        //------------------------------------------------------------------
        public static void Add (Action action)
        {
            Instance.processes.Add (action);
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