using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class DensityParameters
    {
        [SerializeField] private RenderTexture densityNoise = null;
        [SerializeField] private int densityNoiseSize;
        [SerializeField] private ComputeShader densityShader;
        public RenderTexture DensityNoise
        {
            get
            {
                if (densityNoise == null)
                {
                    densityNoise = TextureUtilities.CreateRenderTexture3d(densityNoiseSize, densityNoiseSize, densityNoiseSize);
                    densityNoise = ComputeShaderUtilities.ComputeTexture3d(densityShader, "CSMain", densityNoiseSize, densityNoiseSize, densityNoiseSize, densityNoise);
                    var tmp = densityNoise.depth;
                }
                return densityNoise;
            }
        }
            
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