using UnityEngine;
using Assets.Scripts.Models;

public static class MaterialParametrizer
{
    public static void Parametrize(Material material, CameraParameters cameraParameters)
    {
        material.SetMatrix("_CamToWorld", cameraParameters.CamToWorld);
        material.SetMatrix("_CamInverseProjection", cameraParameters.CamInverseProjection);
        material.SetVector("_CamPos", cameraParameters.CamPos);
    }

    public static void Parametrize(Material material, LightParameters lightParameters)
    {
        material.SetVector("_LightColor", lightParameters.LightColor);
        material.SetVector("_LightPosition", lightParameters.LightPosition);
    }

    public static void Parametrize(Material material, CylinderParameters cylinderParameters)
    {
        material.SetVectorArray("_Cylinders", cylinderParameters.Cylinders);
        material.SetInt("_CylinderCount", cylinderParameters.CylinderCount);
    }

    public static void Parametrize(Material material, ShapeParameters shapeParameters)
    {
        material.SetFloat("_RadiusMultiplier", shapeParameters.RadiusMultiplier);
        material.SetFloat("_HeightMultiplier", shapeParameters.HeightMultiplier);
        material.SetFloat("_RadiusThreshold", shapeParameters.RadiusThreshold);
    }

    public static void Parametrize(Material material, DensityParameters densityParameters)
    {
        material.SetTexture("_DensityNoise", densityParameters.DensityNoise);
        material.SetInt("_DensityNoiseSize", densityParameters.DensityNoiseSize);
    }

    public static void Parametrize(Material material, AreaParameters areaParameters)
    {
        material.SetVector("_areaMin", areaParameters.AreaMin);
        material.SetVector("_areaMax", areaParameters.AreaMax);
    }
}
