namespace Traffic.Cars.Weights
{
    public class Light : Weight
    {
        //------------------------------------------------------------------
        public Light ()
        {
            Lives = 1;
        }

        //------------------------------------------------------------------
        public override string TextureSuffix
        {
            get { return " (Light)"; }
        }
    }
}