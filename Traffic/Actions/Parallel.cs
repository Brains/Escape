using System.Collections.Generic;
using System.Linq;

namespace Traffic.Actions
{
    public class Parallel : Composite
    {
        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            Actions.ForEach (action => action.Update (elapsed));
            
            Actions.RemoveAll (action => action.Finished);
            
            Finished = Actions.Count == 0;
        }
    }
}