using System;
using System.IO;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CylinderRenderer : MonoBehaviour
{
    [Header("Shared Config (ScriptableObject — same asset in all scenes)")]
    public CloudConfigScriptable cloudConfigScriptable;

    [Space(5)]
    [Header("Local Override (per-scene, no shared asset needed)")]
    public CloudConfig cloudConfig;

    [Space(5)]
    [Header("Material")]
    public Material raymarchMat;

    private Camera _camera;
    private Matrix4x4 _lastProjectionMatrix;

    private CloudConfig CloudConfig => cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;

    private void Setup()
    {
        _camera = GetComponent<Camera>();
        _lastProjectionMatrix = _camera.projectionMatrix;
        CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;
        cfg.CameraParameters = new CameraParameters(_camera);
        raymarchMat.Parametrize(cfg);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () => { if (this != null) UpdateMaterial(); };
#endif
    }

    private void Start() => Setup();
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            var dt = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string subfolderPath = Path.Combine(Application.dataPath, "Screenshots");
            // subfolderPath = $"{Application.dataPath}/Screenshots";
            
            if (!Directory.Exists(subfolderPath)) Directory.CreateDirectory(subfolderPath);
            
            string finalPath = Path.Combine(subfolderPath, $"{dt}.png");
            // finalPath = $"{subfolderPath}/{}"
            
            ScreenCapture.CaptureScreenshot(finalPath, 1); 
            Debug.Log($"{dt}: Frame captured");
        }
    }
    private void LateUpdate()
    {
        bool transformChanged = transform.hasChanged;
        bool projectionChanged = _camera.projectionMatrix != _lastProjectionMatrix;
        if (!transformChanged && !projectionChanged) return;
        if (transformChanged) transform.hasChanged = false;
        if (projectionChanged) _lastProjectionMatrix = _camera.projectionMatrix;
        if (transformChanged || projectionChanged)
        {
            CloudConfig cfg = CloudConfig;
            cfg.CameraParameters = new CameraParameters(_camera);
            raymarchMat.Parametrize(cfg.CameraParameters);
        }
    }

    private void UpdateMaterial()
    {
        CloudConfig cfg = CloudConfig;
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
