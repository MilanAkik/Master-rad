using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class DensityParameters
    {
        [System.NonSerialized] private RenderTexture densityNoise = null;
        [SerializeField] private int densityNoiseSize;
        [SerializeField] private ComputeShader densityShader;
        public RenderTexture DensityNoise
        {
            get
            {
                if (densityNoise == null || !densityNoise.IsCreated() || densityNoise.width != densityNoiseSize || densityNoise.height != densityNoiseSize || densityNoise.volumeDepth != densityNoiseSize)
                {
                    ReleaseDensityNoise();
                    densityNoise = TextureUtilities.CreateRenderTexture3d(densityNoiseSize, densityNoiseSize, densityNoiseSize);
                    densityNoise = ComputeShaderUtilities.ComputeTexture3d(densityShader, "CSMain", densityNoiseSize, densityNoiseSize, densityNoiseSize, densityNoise);
                }
                return densityNoise;
            }
        }

        private void ReleaseDensityNoise()
        {
            if (densityNoise == null) return;
            densityNoise.Release();
            if (Application.isPlaying) Object.Destroy(densityNoise);
            else Object.DestroyImmediate(densityNoise);
            densityNoise = null;
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
