namespace Traffic.Cars.Weights
{
    internal class Heavy : Weight
    {
        //------------------------------------------------------------------
        public Heavy (Car car) : base (car)
        {
            this.car.Lives = 3;
            this.car.Acceleration *= 0.5f;
            this.car.Deceleration *= 0.5f;
        }

        //------------------------------------------------------------------
        public override string TextureSuffix
        {
            get { return " (Heavy)"; }
        }
    }
}