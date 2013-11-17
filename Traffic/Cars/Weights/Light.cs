namespace Traffic.Cars.Weights
{
    internal class Light : Weight
    {
        //------------------------------------------------------------------
        public Light (Car car) : base (car)
        {
            Lives = 1;
            this.car.Acceleration *= 2;
            this.car.Deceleration *= 2;
        }

        //------------------------------------------------------------------
        public override string TextureSuffix
        {
            get { return " (Light)"; }
        }
    }
}