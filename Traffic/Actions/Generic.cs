namespace Traffic.Actions
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
        public override void Update (float elapsed)
        {
            action.Invoke ();

            Finished = true;
        }

    }
}