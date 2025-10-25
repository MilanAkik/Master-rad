using Assets.Scripts.Generators;
using UnityEngine;
using static UnityEngine.FilterMode;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CylinderRenderer : MonoBehaviour
{
    public Material raymarchMat;
    
    //Light parameters
    public Color LightColor;
    public Vector3 LightPosition;

    //Cylinder shape parameters
    [Range(0.1f, 5.0f)]
    public float radiusMultiplier = 1;
    [Range(0.1f, 5.0f)]
    public float heightMultiplier = 1;
    [Range(0.1f, 0.9f)]
    public float radiusThreshold = 0.1f;

    //Density shader parameters
    public ComputeShader computeShader;
    public int DensityResolution = 256;
    private RenderTexture resultTexture;

    //Area parameters
    public Vector3 areaMin = new Vector3(-10, -10, -10);
    public Vector3 areaMax = new Vector3( 10,  10,  10);

    //Cylinder parameters
    [Range(1, 128)]
    public int CylinderCount = 3;
    [Range(1,10000)]
    public int RandomSeed = 1234;
    [SerializeField]
    private CylinderGenerator generator;
    private Vector4[] _cylinders = new Vector4[512];

    private void OnValidate()
    {
        resultTexture = new RenderTexture(DensityResolution, DensityResolution, 0)
        {
            enableRandomWrite = true,
            volumeDepth = DensityResolution,
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            filterMode = Point
        };
        resultTexture.Create();

        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetInt("width", DensityResolution);
        computeShader.SetInt("height", DensityResolution);
        computeShader.SetInt("depth", DensityResolution);
        computeShader.SetTexture(kernel, "Result", resultTexture);

        computeShader.Dispatch(kernel, DensityResolution / 8, DensityResolution / 8, DensityResolution / 8);

        var tmp = resultTexture.depth;
        var cyl = generator.getCylinders(CylinderCount, RandomSeed);
        for (int i = 0; i < cyl.Length; i++)
        {
            _cylinders[i] = cyl[i];
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (raymarchMat != null)
        {
            //_cylinders[0] = new Vector4( 0, -2, 5, 0.25f);
            //_cylinders[1] = new Vector4(-2, -0.5f, 10, 0.5f);
            //_cylinders[2] = new Vector4( 2, 1.5f, 7, 0.75f);
            Camera cam = Camera.current ?? Camera.main;
            
            // Camera parameters
            raymarchMat.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
            raymarchMat.SetMatrix("_CamInverseProjection", cam.projectionMatrix.inverse);
            raymarchMat.SetVector("_CamPos", cam.transform.position);

            // Light parmeters
            raymarchMat.SetVector("_LightColor", FromColor(LightColor));
            raymarchMat.SetVector("_LightPosition", LightPosition);

            // Cylinder parameters
            raymarchMat.SetVectorArray("_Cylinders", _cylinders);
            raymarchMat.SetInt("_CylinderCount", CylinderCount);

            //Cylinder shape parameters
            raymarchMat.SetFloat("_RadiusMultiplier", radiusMultiplier);
            raymarchMat.SetFloat("_HeightMultiplier", heightMultiplier);
            raymarchMat.SetFloat("_RadiusThreshold", radiusThreshold);

            //Density parameters
            raymarchMat.SetTexture("_DensityNoise", resultTexture);
            raymarchMat.SetInt("_DensityNoiseSize", DensityResolution);

            //Area parameters
            raymarchMat.SetVector("_areaMin", areaMin);
            raymarchMat.SetVector("_areaMax", areaMax);

            Graphics.Blit(source, destination, raymarchMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private Vector4 FromColor(Color color) => new Vector4(color.r, color.g, color.b, 1f);

}
