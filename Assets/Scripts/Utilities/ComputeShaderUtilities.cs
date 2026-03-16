using System.Drawing;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class ComputeShaderUtilities
    {
        public static RenderTexture ComputeTexture2d(ComputeShader computeShader, string kernelName, int width, int height, RenderTexture resultTexture)
        {
            int kernel = computeShader.FindKernel(kernelName);
            computeShader.SetInt("width", width);
            computeShader.SetInt("height", height);
            computeShader.SetTexture(kernel, "Result", resultTexture);
            computeShader.Dispatch(kernel, width / 8, height / 8, 1);
            return resultTexture;
        }

        public static RenderTexture ComputeTexture3d(ComputeShader computeShader, string kernelName, int width, int height, int depth, RenderTexture resultTexture)
        {
            int kernel = computeShader.FindKernel(kernelName);
            computeShader.SetInt("width", width);
            computeShader.SetInt("height", height);
            computeShader.SetInt("depth", depth);
            computeShader.SetTexture(kernel, "Result", resultTexture);
            computeShader.Dispatch(kernel, width / 8, height / 8, depth / 8);
            return resultTexture;
        }
    }
}
