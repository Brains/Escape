using System.Collections.Generic;

namespace Traffic.Actions
{
    public abstract class Composite : Action
    {
        protected List<Action> Actions;

        //------------------------------------------------------------------
        protected Composite ()
        {
            Actions = new List <Action> ();
        }

        //------------------------------------------------------------------
        public void Add (Action action)
        {
            Actions.Add (action);
        }

    }
}