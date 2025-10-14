using UnityEngine;
using static UnityEngine.FilterMode;

namespace Assets.Scripts.Generators
{
    [CreateAssetMenu(menuName = "CylinderGenerator/Noise")]
    internal class NoiseCylinderGenerator : CylinderGenerator
    {
        public ComputeShader computeShader;
        public int textureSize = 128;
        private RenderTexture resultTexture;
        public override Vector4[] getCylinders(params object[] objects)
        {
            int count = (int)objects[0];
            int seed = (int)objects[1];
            resultTexture = new RenderTexture(textureSize, textureSize, 0) { enableRandomWrite=true, filterMode=Point};
            resultTexture.Create();

            int kernel = computeShader.FindKernel("CSMain");
            computeShader.SetInt("width", textureSize);
            computeShader.SetInt("height", textureSize);
            computeShader.SetTexture(kernel, "Result", resultTexture);

            computeShader.Dispatch(kernel, textureSize / 8, textureSize / 8, 1);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = resultTexture;
            Texture2D tex = new Texture2D(resultTexture.width, resultTexture.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = previous;

            Vector4[] res = new Vector4[count];
            for (int e = 0; e < count; e++)
            {
                float max = 0;
                int maxi = -1;
                int maxj = -1;
                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        var curr = tex.GetPixel(i, j).r;
                        if (curr > max)
                        {
                            max = curr;
                            maxi = i;
                            maxj = j;
                        }
                    }
                }
                float x = ((float)maxi / (float)textureSize) * 2f - 1f;
                float y = tex.GetPixel((maxi + 2) % textureSize, (maxj + 2) % textureSize).r;
                float z = ((float)maxj / (float)textureSize) * 2f - 1f;
                float r = tex.GetPixel((maxi + 5) % textureSize, (maxj + 5) % textureSize).r; ;
                int radius = 8;
                for (int i = -radius; i < radius+1; i++)
                {
                    for (int j = -radius; j < radius+1; j++)
                    {
                        if (i * i + j * j < radius * radius)
                        {
                            var dist = Mathf.Sqrt(i * i + j * j);
                            var oldCol = tex.GetPixel(maxi + i, maxj + j);
                            var oldVal = oldCol.r;
                            var newVal = oldVal * (dist/(float)radius);
                            tex.SetPixel(maxi + i, maxj + j, new Color(newVal, newVal, newVal, 1.0f));
                        }
                    }
                }
                Debug.LogError(max);
                Debug.LogError(maxi);
                Debug.LogError(maxj);
                res[e] = new Vector4(x, y, z, r);
            }
            return res;
        }
    }
}
