using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffic.Actions.Base
{
    public class Loop : Sequence
    {
        private readonly List <Action> looped;

        //------------------------------------------------------------------
        public Loop ()
        {
            looped = new List <Action> ();
        }

        //------------------------------------------------------------------
        public override Action Copy ()
        {
            throw new NotImplementedException();
        }

        //-----------------------------------------------------------------
        public void AddLooped (Action action)
        {
            looped.Add (action.Copy ());
            Add (action);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            if (Finished) 
                Restart ();
            
            base.Update (elapsed);
        }


        //------------------------------------------------------------------
        private void Restart ()
        {
            foreach (var action in looped)
                Add (action.Copy ());

            Finished = false;
        }
    }
}