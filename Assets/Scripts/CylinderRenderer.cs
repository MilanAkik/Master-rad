using NUnit.Framework.Internal;
using System;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CylinderRenderer : MonoBehaviour
{
    public Material raymarchMat;
    public Color LightColor;
    public Vector3 LightPosition;

    private Vector4[] _cylinders = new Vector4[512];

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (raymarchMat != null)
        {
            _cylinders[0] = new Vector4( 0, 0, 2, 0.25f);
            _cylinders[1] = new Vector4(-2, 0, 4, 0.5f);
            _cylinders[2] = new Vector4( 2, 0, 3, 0.75f);
            Camera cam = Camera.current ?? Camera.main;
            //Debug.LogError(cam.transform.position);
            //Debug.LogError(cam.cameraToWorldMatrix);
            //Debug.LogError(cam.projectionMatrix.inverse);
            //var res = cam.projectionMatrix * new Vector4(-1, -1, -1, 1);
            //res = res / res.w;
            //var n = new Vector4(res.x, res.y, res.z, 0);
            //var o = cam.cameraToWorldMatrix * n;
            //Debug.LogError(o.normalized);

            // Camera parameters
            raymarchMat.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
            raymarchMat.SetMatrix("_CamInverseProjection", cam.projectionMatrix.inverse);
            raymarchMat.SetVector("_CamPos", cam.transform.position);

            // Light parmeters
            raymarchMat.SetVector("_LightColor", FromColor(LightColor));
            raymarchMat.SetVector("_LightPosition", LightPosition);

            // Cylinder equation (((x-1)/(2)))^(2)+(((y-1)/(2)))^(2)<1
            // Cylinder parameters
            raymarchMat.SetVectorArray("_Cylinders", _cylinders);
            raymarchMat.SetInt("_CylinderCount", 3);

            Graphics.Blit(source, destination, raymarchMat);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private Vector4 FromColor(Color color) => new Vector4(color.r, color.g, color.b, 1f);

}
