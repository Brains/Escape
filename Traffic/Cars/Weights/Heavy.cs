namespace Traffic.Cars.Weights
{
    internal class Heavy : Weight
    {
        //------------------------------------------------------------------
        public Heavy () : base ()
        {
            Lives = 3;
            Acceleration = 0.5f;
            Deceleration = 0.5f;
            TextureSuffix = "(Heavy)";
        }
    }
}