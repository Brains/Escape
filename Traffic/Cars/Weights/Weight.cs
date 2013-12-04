namespace Traffic.Cars.Weights
{
    public abstract class Weight
    {
        public int Lives { get; set; }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }
        public string TextureSuffix { get; set; }
    }
}