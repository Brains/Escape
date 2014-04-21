using System.Linq;

namespace Engine.Actions
{
    public class Sequence : Composite
    {
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