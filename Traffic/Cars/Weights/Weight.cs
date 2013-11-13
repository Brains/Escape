namespace Traffic.Cars.Weights
{
    public abstract class Weight
    {
        public int Lives { get; set; }
        public abstract string TextureSuffix { get; }
    }
}