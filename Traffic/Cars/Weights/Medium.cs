namespace Traffic.Cars.Weights
{
    internal class Medium : Weight
    {
        //------------------------------------------------------------------
        public Medium () : base ()
        {
            Lives = 2;
            Acceleration = 1;
            Deceleration = 1;
            TextureSuffix = "(Medium)";
        }
    }
}