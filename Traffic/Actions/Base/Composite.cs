using System.Collections.Generic;

namespace Traffic.Actions.Base
{
    public abstract class Composite : Action
    {
        public List <Action> Actions { get; set; }

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