using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    [CreateAssetMenu(menuName = "CylinderGenerator/Noise")]
    internal class NoiseCylinderGenerator : CylinderGenerator
    {
        public ComputeShader computeShader;
        public int textureSize = 128;
        private RenderTexture resultTexture;
        private Vector2Int _yOffset = new Vector2Int(2, 2);
        private Vector2Int _rOffset = new Vector2Int(5, 5);
        private Vector2Int _hOffset = new Vector2Int(3, 3);

        public override Vector4[] getCylinders(GeneratorParameters parameters)
        {
            int count = parameters.CylinderCount;
            int seed = parameters.RandomSeed;
            resultTexture = TextureUtilities.CreateRenderTexture2d(textureSize, textureSize);
            resultTexture = ComputeShaderUtilities.ComputeTexture2d(computeShader, "CSMain", textureSize, textureSize, resultTexture);

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
                            var newVal = oldVal * 0.8f * (dist/(float)radius);
                            tex.SetPixel(maxi + i, maxj + j, new Color(newVal, newVal, newVal, 1.0f));
                        }
                    }
                }
                if (x == 0) x = 0.001f;
                if (y == 0) y = 0.001f;
                if (z == 0) z = 0.001f;
                if (r == 0) r = 0.001f;
                res[e] = new Vector4(2f*x, 1f-y, 2f*z+8f, r);
            }
            return res;
        }

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters)
        {
            int count = parameters.CylinderCount;
            int seed = parameters.RandomSeed;
            
            resultTexture = TextureUtilities.CreateRenderTexture2d(textureSize, textureSize);
            resultTexture = ComputeShaderUtilities.ComputeTexture2d(computeShader, "CSMain", textureSize, textureSize, resultTexture);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = resultTexture;
            Texture2D tex = new Texture2D(resultTexture.width, resultTexture.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = previous;

            var res = new Matrix4x4[count];
            for (int e = 0; e < count; e++)
            {
                var (maxi, maxj) = getMaxValue(tex);
                float x = ((float)maxi / (float)textureSize) * 2f - 1f;
                float y = tex.GetPixel((maxi + _yOffset.x) % textureSize, (maxj + _yOffset.y) % textureSize).r;
                float z = ((float)maxj / (float)textureSize) * 2f - 1f;
                float r = tex.GetPixel((maxi + _rOffset.x) % textureSize, (maxj + _rOffset.y) % textureSize).r;
                float h = tex.GetPixel((maxi + _hOffset.x) % textureSize, (maxj + _hOffset.y) % textureSize).r;
                int radius = 8;
                for (int i = -radius; i < radius + 1; i++)
                {
                    for (int j = -radius; j < radius + 1; j++)
                    {
                        if (i * i + j * j < radius * radius)
                        {
                            var dist = Mathf.Sqrt(i * i + j * j);
                            var oldCol = tex.GetPixel(maxi + i, maxj + j);
                            var oldVal = oldCol.r;
                            var newVal = oldVal * 0.8f * (dist / (float)radius);
                            tex.SetPixel(maxi + i, maxj + j, new Color(newVal, newVal, newVal, 1.0f));
                        }
                    }
                }
                if (x == 0) x = 0.001f;
                if (y == 0) y = 0.001f;
                if (z == 0) z = 0.001f;
                if (r == 0) r = 0.001f;
                if (h == 0) h = 0.001f;
                x = 2f * x;
                y = 1f - y;
                z = 2f * z + 8f;
                r = r * parameters.RadiusMultiplier;
                h = h * parameters.HeightMultiplier;
                res[e] = new Matrix4x4( new Vector4(x, y, z, r), new Vector4(h, 0, 0, 0), Vector4.zero, Vector4.zero );
            }
            return res;
        }

        private (int, int) getMaxValue(Texture2D tex)
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
            return (maxi, maxj);
        }

    }
}
