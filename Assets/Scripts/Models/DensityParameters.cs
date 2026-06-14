using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class DensityParameters
    {
        [SerializeField] private RenderTexture densityNoise;
        [SerializeField] private int densityNoiseSize;
        [SerializeField] private ComputeShader densityShader;
        public RenderTexture DensityNoise => densityNoise;
        public int DensityNoiseSize => densityNoiseSize;
        public ComputeShader DensityShader => densityShader;

        public DensityParameters(RenderTexture densityNoise, int densityNoiseSize, ComputeShader densityShader)
        {
            this.densityNoise = densityNoise;
            this.densityNoiseSize = densityNoiseSize;
            this.densityShader = densityShader; 
        }
    }
}