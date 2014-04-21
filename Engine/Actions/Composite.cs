using System.Collections.Generic;

namespace Engine.Actions
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
        public virtual void Add (Action action)
        {
            Actions.Add (action);
        }
    }
}