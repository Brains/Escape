using System.Collections.Generic;
using System.Linq;

namespace Traffic.Actions.Base
{
    public class Loop : Composite
    {
        private List<Action>.Enumerator enumerator;
        private readonly Dictionary<Action, Action> startPoints;

        //------------------------------------------------------------------
        public Loop ()
        {
            startPoints = new Dictionary<Action, Action> ();
        }

        //-----------------------------------------------------------------
        public new void Add (Action action)
        {
            base.Add (action);

            // Add Start Point for Composite Action
            Composite composite = action as Composite;

            if (composite != null)
                startPoints.Add (composite, composite.Actions.First ());
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            // Create enumerator
            if (enumerator.Current == null)
            {
                enumerator = Actions.GetEnumerator ();
                enumerator.MoveNext ();
            }

            // Update current Sequence
            enumerator.Current.Update (elapsed);

            if (enumerator.Current.Finished)
                FinishAction (enumerator.Current);
        }

        //------------------------------------------------------------------
        // Finish Sequence so we can start it on next iteration
        private void FinishAction (Action action)
        {
            Composite composite = action as Composite;

            if (composite != null)
                composite.Add (startPoints[action]);

            action.Finished = false;
            enumerator.MoveNext ();
        }
    }
}