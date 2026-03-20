using UnityEngine;

namespace Assets.Scripts.Models
{
    public class CameraParameters
    {
        public Matrix4x4 CamToWorld { get; set; }
        public Matrix4x4 CamInverseProjection { get; set; }
        public Vector3 CamPos { get; set; }
        public CameraParameters(Matrix4x4 camToWorld, Matrix4x4 camInverseProjection, Vector3 camPos)
        {
            CamToWorld = camToWorld;
            CamInverseProjection = camInverseProjection;
            CamPos = camPos;
        }
    }
}