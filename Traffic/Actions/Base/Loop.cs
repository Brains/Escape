using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffic.Actions.Base
{
    public class Loop : Sequence
    {
        private List<Action>.Enumerator enumerator;

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            if (enumerator.Current == null)
                Reset();

            // Update current Sequence
            enumerator.Current.Update (elapsed);

            if (enumerator.Current.Finished)
                enumerator.MoveNext ();
        }

        //------------------------------------------------------------------
        public override void Reset ()
        {
            enumerator = Actions.GetEnumerator ();
            enumerator.MoveNext ();

            Actions.ForEach (action => action.Reset());
        }
    }
}