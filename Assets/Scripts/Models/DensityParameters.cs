using UnityEngine;

public class DensityParameters
{
    public RenderTexture DensityNoise { get; set; }
    public int DensityNoiseSize { get; set; }
    public DensityParameters(RenderTexture densityNoise, int densityNoiseSize)
    {
        DensityNoise = densityNoise;
        DensityNoiseSize = densityNoiseSize;
    }
}
