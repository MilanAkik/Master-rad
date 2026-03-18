using UnityEngine;

public class LightParameters
{
    Vector4 LightColor { get; set; }
    Vector3 LightPos { get; set; }
    public LightParameters(Vector4 lightColor, Vector3 lightPos)
    {
        LightColor = lightColor;
        LightPos = lightPos;
    }
}
