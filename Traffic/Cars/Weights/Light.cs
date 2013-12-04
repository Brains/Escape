namespace Traffic.Cars.Weights
{
    internal class Light : Weight
    {
        //------------------------------------------------------------------
        public Light () : base ()
        {
            Lives = 1;
            Acceleration = 2;
            Deceleration = 2;
            TextureSuffix = "(Light)";
        }
    }
}