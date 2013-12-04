namespace Traffic
{
    public static class ControlCenter
    {
        // Cars Generation
        public static bool NoCars;
        public static bool NoPolice;
        public static bool NoBlocks;
        public const int MaximumCarsOnLane = 8;

        // Player
        public static bool NoPlayerAdjustSpeed;

        //------------------------------------------------------------------
        static ControlCenter()
        {
            NoCars = true;
//            NoPolice = true;

            NoPlayerAdjustSpeed = true;
        }
    }
}