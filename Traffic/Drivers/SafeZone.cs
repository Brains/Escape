namespace Traffic.Drivers
{
    public class SafeZone
    {
        private readonly Driver driver;

        //------------------------------------------------------------------
        public SafeZone (Driver driver, float scale)
        {
            this.driver = driver;
            Scale = scale;
        }

        //------------------------------------------------------------------
        public float Scale { get; set; }

        //------------------------------------------------------------------
        public float HighDanger
        {
            get { return Calculate (0.33f); }
        }
        
        //------------------------------------------------------------------
        public float MediumDanger
        {
            get { return Calculate (0.66f); }
        }
        
        //------------------------------------------------------------------
        public float LowDanger
        {
            get { return Calculate (1.0f); }
        }

        //------------------------------------------------------------------
        private float Calculate (float fraction)
        {
            const float normalize = 0.7f;
            float velocity = driver.Car.Velocity * normalize ;

            return driver.Car.Lenght + (velocity * fraction * Scale);
        }
    }
}