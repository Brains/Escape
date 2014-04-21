namespace Engine.Actions
{
    public class SequenceInitial : Sequence
    {
        public Action Initial;

        //------------------------------------------------------------------
        public override void Reset()
        {
            base.Reset();

            Initial.Reset();
            Add (Initial);
        }
    }
}