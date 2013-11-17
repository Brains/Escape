namespace Traffic.Cars.Weights
{
    internal abstract class Weight
    {
        protected Car car;
        public int Lives { get; set; }
        public abstract string TextureSuffix { get; }

        //------------------------------------------------------------------
        protected Weight (Car car)
        {
            this.car = car;
        }
    }
}