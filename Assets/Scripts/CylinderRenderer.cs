using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CylinderRenderer : MonoBehaviour
{
    [Header("Shared Config (ScriptableObject — same asset in all scenes)")]
    public CloudConfigScriptable cloudConfigScriptable;

    [Header("Local Override (per-scene, no shared asset needed)")]
    public CloudConfig cloudConfig;

    [Header("Material")]
    public Material raymarchMat;

    [Space(5)]
    private RenderTexture resultTexture;

    private void Setup()
    {
        CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;
        var densityResolution = cfg.DensityParameters.DensityNoiseSize;
        var computeShader = cfg.DensityParameters.DensityShader;
        resultTexture = TextureUtilities.CreateRenderTexture3d(densityResolution, densityResolution, densityResolution);
        resultTexture = ComputeShaderUtilities.ComputeTexture3d(computeShader, "CSMain", densityResolution, densityResolution, densityResolution, resultTexture);
        var tmp = resultTexture.depth;
    }

    private void OnValidate() => Setup();

    private void Start() => Setup();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (raymarchMat != null)
        {
            CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;
            cfg.CameraParameters = new CameraParameters(Camera.current ?? Camera.main);
            raymarchMat.Parametrize(cfg);
            Graphics.Blit(source, destination, raymarchMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
