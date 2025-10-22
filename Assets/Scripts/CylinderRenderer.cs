using NUnit.Framework.Internal;
using System;
using UnityEngine;

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

    private Vector4[] _cylinders = new Vector4[512];

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (raymarchMat != null)
        {
            _cylinders[0] = new Vector4( 0, -2, 10, 0.25f);
            _cylinders[1] = new Vector4(-2, -0.5f, 20, 0.5f);
            _cylinders[2] = new Vector4( 2, 1.5f, 15, 0.75f);
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

            Graphics.Blit(source, destination, raymarchMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private Vector4 FromColor(Color color) => new Vector4(color.r, color.g, color.b, 1f);

}
