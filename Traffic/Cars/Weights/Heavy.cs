namespace Traffic.Cars.Weights
{
    public class Heavy : Weight
    {
        //------------------------------------------------------------------
        public Heavy ()
        {
            Lives = 3;
        }

        //------------------------------------------------------------------
        public override string TextureSuffix
        {
            get { return " (Heavy)"; }
        }
    }
}