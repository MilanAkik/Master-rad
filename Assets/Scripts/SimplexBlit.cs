using Assets.Scripts.Utilities;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class SimplexBlit : MonoBehaviour
{
    public ComputeShader computeShader;
    public int textureSize = 128;
    private RenderTexture resultTexture;

    private void OnValidate()
    {
        resultTexture = TextureUtilities.CreateRenderTexture2d(textureSize, textureSize);
        resultTexture = ComputeShaderUtilities.ComputeTexture2d(computeShader, "CSMain", textureSize, textureSize, resultTexture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(resultTexture, dest);
    }
}