using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class DensityParameters
    {
        [SerializeField] private RenderTexture densityNoise;
        [SerializeField] private int densityNoiseSize;
        public RenderTexture DensityNoise => densityNoise;
        public int DensityNoiseSize => densityNoiseSize;

        public DensityParameters(RenderTexture densityNoise, int densityNoiseSize)
        {
            this.densityNoise = densityNoise;
            this.densityNoiseSize = densityNoiseSize;
        }
    }
}