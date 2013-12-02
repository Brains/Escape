using System.Collections.Generic;

namespace Traffic.Actions.Base
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