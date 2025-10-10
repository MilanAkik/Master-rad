using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RaymarchRenderer : MonoBehaviour
{
    public Material raymarchMat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        
        if (raymarchMat != null)
        {
            Camera cam = Camera.current ?? Camera.main;

            // Send matrices and camera position
            raymarchMat.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
            raymarchMat.SetMatrix("_CamInverseProjection", cam.projectionMatrix.inverse);
            raymarchMat.SetVector("_CamPos", cam.transform.position);

            Graphics.Blit(src, dest, raymarchMat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}