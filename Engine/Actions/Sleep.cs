namespace Engine.Actions
{
    public class Sleep : Action
    {
        //------------------------------------------------------------------
        public Sleep (float duration) : base (duration) {}

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            Elapsed += elapsed;

            if (Elapsed >= Duration)
                Finished = true;
        }

    }
}