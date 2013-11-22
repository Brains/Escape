using System;

namespace Traffic.Actions.Base
{
    public class Conditional : Action
    {
        private readonly Func <bool> condition;
        private readonly System.Action action;

        //------------------------------------------------------------------
        public Conditional (System.Func <bool> condition, System.Action action)
        {
            this.condition = condition;
            this.action = action;
        }

        //------------------------------------------------------------------
        public override Action Copy ()
        {
            return new Conditional (condition,action);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            if (condition.Invoke ()) 
                action.Invoke ();

            Finished = true;
        }
    }
}