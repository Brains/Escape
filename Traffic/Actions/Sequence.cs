using System.Collections.Generic;
using System.Linq;

namespace Traffic.Actions
{
    public class Sequence : Composite
    {
        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            Finished = Actions.Count == 0;
            
            if (Actions.Count <= 0) return;

            Actions.First ().Update (elapsed);

            if (Actions.First ().Finished)
                Actions.Remove (Actions.First ());
        }
    }
}