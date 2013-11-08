namespace Traffic.Actions.Base
{
    public class Loop : Action
    {
        private readonly System.Action action;

        //------------------------------------------------------------------
        public Loop (System.Action action)
        {
            this.action = action;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            action.Invoke ();
        }

    }
}