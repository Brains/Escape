using System;

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
        public float Calculate (float fraction)
        {
            const float normalize = 0.5f;
            float velocity = (float) ((driver.Car.Velocity) * normalize) ;

            return driver.Car.Lenght * 1.5f + (velocity * fraction * Scale);
        }
    }
}