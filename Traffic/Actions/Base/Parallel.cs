using System.Collections.Generic;

namespace Traffic.Actions.Base
{
    public class Parallel : Composite
    {
        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            var actions = new List <Action> ();
            actions.ForEach (action => action.Update (elapsed));
            
            actions.RemoveAll (action => action.Finished);
            
            Finished = actions.Count == 0;
        }
    }
}