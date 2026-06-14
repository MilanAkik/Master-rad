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
    private Camera _camera;
    private Matrix4x4 _lastProjectionMatrix;

    private void Setup()
    {
        CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;
        var densityResolution = cfg.DensityParameters.DensityNoiseSize;
        var computeShader = cfg.DensityParameters.DensityShader;
        resultTexture = TextureUtilities.CreateRenderTexture3d(densityResolution, densityResolution, densityResolution);
        resultTexture = ComputeShaderUtilities.ComputeTexture3d(computeShader, "CSMain", densityResolution, densityResolution, densityResolution, resultTexture);
        var tmp = resultTexture.depth;
        _camera = GetComponent<Camera>();
        _lastProjectionMatrix = _camera.projectionMatrix;
        cfg.CameraParameters = new CameraParameters(_camera);
        raymarchMat.Parametrize(cfg);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) UpdateMaterial();
        };
#endif
    }

    private void Start() => Setup();

    private void LateUpdate()
    {
        bool transformChanged = transform.hasChanged;
        bool projectionChanged = _camera.projectionMatrix != _lastProjectionMatrix;
        if (!transformChanged && !projectionChanged) return;
        CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;
        if (transformChanged) transform.hasChanged = false;
        if (projectionChanged) _lastProjectionMatrix = _camera.projectionMatrix;
        cfg.CameraParameters = new CameraParameters(_camera);
        raymarchMat.Parametrize(cfg.CameraParameters);
    }

    private void UpdateMaterial()
    {
        CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;
        raymarchMat.Parametrize(cfg);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (raymarchMat != null)
        {
            Graphics.Blit(source, destination, raymarchMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
