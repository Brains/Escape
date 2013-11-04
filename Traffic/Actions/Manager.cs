using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Traffic.Actions
{
    public class Manager : GameComponent
    {
        private readonly List<Action> actions = new List <Action> ();

        public static Manager Instance;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Instance = this;
        }

        //------------------------------------------------------------------
        public static void Add (Action action)
        {
            Instance.actions.Add (action);
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var process in actions)
            {
                process.Update (elapsed);
            }

            actions.RemoveAll (process => process.Finished);
        }

    }
}