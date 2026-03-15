using System.Drawing;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class ComputeShaderUtilities
    {
        public static RenderTexture ComputeTexture2d(ComputeShader computeShader, string kernelName, int width, int height)
        {
            RenderTexture resultTexture = new RenderTexture(width, height, 0) { enableRandomWrite = true, filterMode = FilterMode.Point };
            resultTexture.Create();
            int kernel = computeShader.FindKernel(kernelName);
            computeShader.SetInt("width", width);
            computeShader.SetInt("height", height);
            computeShader.SetTexture(kernel, "Result", resultTexture);

            computeShader.Dispatch(kernel, width / 8, height / 8, 1);

            return resultTexture;
        }
    }
}
