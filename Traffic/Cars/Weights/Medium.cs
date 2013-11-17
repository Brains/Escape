namespace Traffic.Cars.Weights
{
    internal class Medium : Weight
    {
        //------------------------------------------------------------------
        public Medium (Car car) : base (car)
        {
            Lives = 2;
            this.car.Acceleration *= 1;
            this.car.Deceleration *= 1;
        }

        //------------------------------------------------------------------
        public override string TextureSuffix
        {
            get { return " (Medium)"; }
        } 
    }
}