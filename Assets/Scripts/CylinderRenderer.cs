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
    public float radiusMultiplier = 1;
    public float heightMultiplier = 1;

    //Density shader parameters
    public ComputeShader computeShader;
    public int DensityResolution = 256;
    private RenderTexture resultTexture;

    //Area parameters
    public Vector3 areaMin = new Vector3(-10, -10, -10);
    public Vector3 areaMax = new Vector3( 10,  10,  10);

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
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (raymarchMat != null)
        {
            _cylinders[0] = new Vector4( 0, -2, 5, 0.25f);
            _cylinders[1] = new Vector4(-2, -0.5f, 10, 0.5f);
            _cylinders[2] = new Vector4( 2, 1.5f, 7, 0.75f);
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
            raymarchMat.SetInt("_CylinderCount", 3);

            //Cylinder shape parameters
            raymarchMat.SetFloat("_RadiusMultiplier", radiusMultiplier);
            raymarchMat.SetFloat("_HeightMultiplier", heightMultiplier);

            //Density parameters
            raymarchMat.SetTexture("_DensityNoise", resultTexture);

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
