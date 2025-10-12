using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class SimplexBlit : MonoBehaviour
{
    public ComputeShader computeShader;
    public int textureSize = 128;
    private RenderTexture resultTexture;

    private void OnValidate()
    {
        resultTexture = new RenderTexture(textureSize, textureSize, 0);
        resultTexture.enableRandomWrite = true;
        resultTexture.filterMode = FilterMode.Point;
        resultTexture.Create();

        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetInt("width", textureSize);
        computeShader.SetInt("height", textureSize);
        computeShader.SetTexture(kernel, "Result", resultTexture);

        computeShader.Dispatch(kernel, textureSize / 8, textureSize / 8, 1);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(resultTexture, dest);
    }
}