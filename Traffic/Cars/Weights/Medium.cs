namespace Traffic.Cars.Weights
{
    public class Medium : Weight
    {
        //------------------------------------------------------------------
        public Medium ()
        {
            Lives = 2;
        }

        //------------------------------------------------------------------
        public override string TextureSuffix
        {
            get { return " (Medium)"; }
        } 
    }
}