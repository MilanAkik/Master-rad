using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class CameraParameters
    {
        [SerializeField] private Matrix4x4 camToWorld;
        [SerializeField] private Matrix4x4 camInverseProjection;
        [SerializeField] private Vector3 camPos;
        public Matrix4x4 CamToWorld => camToWorld;
        public Matrix4x4 CamInverseProjection => camInverseProjection;
        public Vector3 CamPos => camPos;

        public CameraParameters(Matrix4x4 camToWorld, Matrix4x4 camInverseProjection, Vector3 camPos)
        {
            this.camToWorld = camToWorld;
            this.camInverseProjection = camInverseProjection;
            this.camPos = camPos;
        }

        public CameraParameters(Camera camera)
            : this(camera.cameraToWorldMatrix, camera.projectionMatrix.inverse, camera.transform.position) { }
    }
}