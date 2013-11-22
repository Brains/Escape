namespace Traffic.Actions.Base
{
    public class Generic : Action
    {
        private readonly System.Action action;

        //------------------------------------------------------------------
        public Generic (System.Action action)
        {
            this.action = action;
        }

        //------------------------------------------------------------------
        public override Action Copy ()
        {
            return new Generic (action);
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            action.Invoke ();

            Finished = true;
        }

    }
}