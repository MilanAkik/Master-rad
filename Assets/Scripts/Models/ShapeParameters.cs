public class ShapeParameters
{
    public float RadiusMultiplier { get; set; }
    public float HeightMultiplier { get; set; }
    public float RadiusThreshold { get; set; }
    public ShapeParameters(float radiusMultiplier, float heightMultiplier, float radiusThreshold)
    {
        RadiusMultiplier = radiusMultiplier;
        HeightMultiplier = heightMultiplier;
        RadiusThreshold = radiusThreshold;
    }
}
