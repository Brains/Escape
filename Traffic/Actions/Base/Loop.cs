namespace Traffic.Actions.Base
{
    public class Loop : Sequence
    {
        private Action initial;

        //------------------------------------------------------------------
        protected Action Initial
        {
            get { return initial; }
            set
            {
                initial = value;
                Add (Initial);
            }
        }

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            base.Update (elapsed);
            
            // Provide looping
            if (Finished)
            {
                Add (Initial);
                Finished = false;
            }
        }

    }
}