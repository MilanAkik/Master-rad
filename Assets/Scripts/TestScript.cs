using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        Material mat = rend.material;
        Camera cam = Camera.current ?? Camera.main;
        mat.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
        mat.SetMatrix("_CamInverseProjection", cam.projectionMatrix.inverse);
        mat.SetVector("_CamPos", cam.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
