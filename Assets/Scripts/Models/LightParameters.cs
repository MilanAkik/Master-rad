using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class LightParameters
    {
        [SerializeField] private Vector4 lightColor = new Vector4(1f, 1f, 1f, 1f);
        [SerializeField] private Vector3 lightPosition = Vector3.zero;
        public Vector4 LightColor => lightColor;
        public Vector3 LightPosition => lightPosition;

        public LightParameters(Vector4 lightColor, Vector3 lightPosition)
        {
            this.lightColor = lightColor;
            this.lightPosition = lightPosition;
        }
        public LightParameters(Color lightColor, Vector3 lightPosition) : this(FromColor(lightColor), lightPosition) { }

        private static Vector4 FromColor(Color color) => new Vector4(color.r, color.g, color.b, 1f);
    }
}