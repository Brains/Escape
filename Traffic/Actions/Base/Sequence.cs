using System.Linq;

namespace Traffic.Actions.Base
{
    public class Sequence : Composite
    {
        //------------------------------------------------------------------
        public override Action Copy ()
        {
            return base.Copy (new Sequence ());
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            if (Actions.Count > 0)
            {
                Actions.First ().Update (elapsed);

                if (Actions.First ().Finished)
                    Actions.Remove (Actions.First ());
            }

            Finished = Actions.Count == 0;
        }
    }
}