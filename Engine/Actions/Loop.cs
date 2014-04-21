using System.Collections.Generic;

namespace Engine.Actions
{
    public class Loop : Sequence
    {
        private List<Action> newActions;
        private List<Action>.Enumerator enumerator;

        //------------------------------------------------------------------
        public Loop()
        {
            newActions = new List <Action>();
        }

        //------------------------------------------------------------------
        public override void Add (Action action)
        {
            newActions.Add (action);
        }

        //------------------------------------------------------------------
        private void AddNewActions ()
        {
            foreach (var action in newActions)
                base.Add (action);
            newActions.Clear ();
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            if (enumerator.Current == null)
                Reset();
            
            // Update current Sequence
            enumerator.Current.Update (elapsed);

            if (enumerator.Current.Finished)
                if (!enumerator.MoveNext ())
                    Reset ();
        }

        //------------------------------------------------------------------
        public override void Reset ()
        {
            AddNewActions ();

            enumerator = Actions.GetEnumerator ();
            enumerator.MoveNext ();

            Actions.ForEach (action => action.Reset());
        }


    }
}