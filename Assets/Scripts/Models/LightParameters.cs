using UnityEngine;

namespace Assets.Scripts.Models
{
    public class LightParameters
    {
        public Vector4 LightColor { get; set; }
        public Vector3 LightPosition { get; set; }
        public LightParameters(Vector4 lightColor, Vector3 lightPosition)
        {
            LightColor = lightColor;
            LightPosition = lightPosition;
        }
        public LightParameters(Color lightColor, Vector3 lightPosition) : this(FromColor(lightColor), lightPosition) { }

        private static Vector4 FromColor(Color color) => new Vector4(color.r, color.g, color.b, 1f);
    }
}