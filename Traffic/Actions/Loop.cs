namespace Traffic.Actions
{
    public class Loop : Action
    {
        private readonly System.Action action;
        private int counter;
        private readonly int times;

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