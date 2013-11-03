namespace Traffic.Actions
{
    public class Repeated : Action
    {
        private readonly System.Action action;
        private int counter;
        private readonly int times;

        //------------------------------------------------------------------
        public Repeated (System.Action action, int times)
        {
            this.action = action;
            this.times = times;
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            action.Invoke ();
            counter++;

            if (counter >= times)
                Finished = true;
        }

    }
}