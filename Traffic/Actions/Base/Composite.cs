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

        //-----------------------------------------------------------------
        public virtual Composite Copy (Composite copy)
        {
            foreach (var action in Actions)
                copy.Add (action.Copy ());

            return copy;
        }

        //------------------------------------------------------------------
        public override string ToString ()
        {
//            string componentsNames = "";
//
//            foreach (var action in Actions)
//                componentsNames += action + "; ";
//
//            return Name + ": " + componentsNames;
            return Name;
        }


    }
}