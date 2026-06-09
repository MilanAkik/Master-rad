using Assets.Scripts.Generators;
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

    //Material parameters
    [Header("Material")]
    public Material raymarchMat;
    [Space(5)]

    //Light parameters
    [Header("Light")]
    public Color LightColor;
    public Vector3 LightPosition;
    [Space(5)]

    //Cylinder shape parameters
    [Header("Cylinder shape")]
    [Range(0.1f, 5.0f)]
    public float radiusMultiplier = 1;
    [Range(0.1f, 5.0f)]
    public float heightMultiplier = 1;
    [Range(0.1f, 0.9f)]
    public float radiusThreshold = 0.1f;
    [Space(5)]

    //Density shader parameters
    [Header("Density shader")]
    public ComputeShader computeShader;
    public int DensityResolution = 256;
    private RenderTexture resultTexture;
    [Space(5)]

    //Area parameters
    [Header("Area")]
    public Vector3 areaMin = new Vector3(-10, -10, -10);
    public Vector3 areaMax = new Vector3( 10,  10,  10);
    [Space(5)]

    //Cylinder parameters
    [Header("Cylinder")]
    [Range(1, 128)]
    public int CylinderCount = 3;
    [Range(1,10000)]
    public int RandomSeed = 1234;
    [SerializeField]
    private CylinderGenerator generator;
    private Vector4[] _cylinders = new Vector4[512];

    private void Setup()
    {
        resultTexture = TextureUtilities.CreateRenderTexture3d(DensityResolution, DensityResolution, DensityResolution);
        resultTexture = ComputeShaderUtilities.ComputeTexture3d(computeShader, "CSMain", DensityResolution, DensityResolution, DensityResolution, resultTexture);
        var tmp = resultTexture.depth;
        var generatorParameters = new GeneratorParameters { CylinderCount = CylinderCount, RandomSeed = RandomSeed };
        var cyl = generator.getCylinders(generatorParameters);
        for (int i = 0; i < cyl.Length; i++)
        {
            _cylinders[i] = cyl[i];
        }
        if (cloudConfigScriptable != null)
        {
            cloudConfigScriptable.cloudConfig.CylinderParameters.Cylinders = _cylinders;
        }
        else
        {
            cloudConfig.CylinderParameters.Cylinders = _cylinders;
        }
    }

    private void OnValidate() => Setup();

    private void Start() => Setup();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (raymarchMat != null)
        {
            CloudConfig cfg = cloudConfigScriptable != null ? cloudConfigScriptable.cloudConfig : cloudConfig;

            var cameraParameters = new CameraParameters(Camera.current ?? Camera.main);
            //var lightParameters = new LightParameters(LightColor, LightPosition);
            //var cylinderParameters = new CylinderParameters(_cylinders, CylinderCount);
            //var shapeParameters = new ShapeParameters(radiusMultiplier, heightMultiplier, radiusThreshold);
            //var densityParameters = new DensityParameters(resultTexture, DensityResolution);
            //var areaParameters = new AreaParameters(areaMin, areaMax);
            //var cameraParameters = cfg.CameraParameters;
            var lightParameters = cfg.LightParameters;
            var cylinderParameters = cfg.CylinderParameters;
            var shapeParameters = cfg.ShapeParameters;
            var densityParameters = cfg.DensityParameters;
            var areaParameters = cfg.AreaParameters;
            raymarchMat.Parametrize(cameraParameters);
            raymarchMat.Parametrize(lightParameters);
            raymarchMat.Parametrize(cylinderParameters);
            raymarchMat.Parametrize(shapeParameters);
            raymarchMat.Parametrize(densityParameters);
            raymarchMat.Parametrize(areaParameters);

            Graphics.Blit(source, destination, raymarchMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
