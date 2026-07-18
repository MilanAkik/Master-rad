using UnityEngine;
using Assets.Scripts.Models;

public static class MaterialParametrizer
{
    private static int PropertyId_CamToWorld = Shader.PropertyToID("_CamToWorld");
    private static int PropertyId_CameraInverseProjection = Shader.PropertyToID("_CamInverseProjection");
    private static int PropertyId_CamPos = Shader.PropertyToID("_CamPos");

    private static int PropertyId_LightColor = Shader.PropertyToID("_LightColor");
    private static int PropertyId_LightPosition = Shader.PropertyToID("_LightPosition");

    private static int PropertyId_Cylinders = Shader.PropertyToID("_Cylinders");
    private static int PropertyId_CylinderMatrices = Shader.PropertyToID("_CylinderMatrices");
    private static int PropertyId_CylinderCount = Shader.PropertyToID("_CylinderCount");

    private static int PropertyId_RadiusMultiplier = Shader.PropertyToID("_RadiusMultiplier");
    private static int PropertyId_HeightMultiplier = Shader.PropertyToID("_HeightMultiplier");
    private static int PropertyId_RadiusThreshold = Shader.PropertyToID("_RadiusThreshold");

    private static int PropertyId_DensityNoise = Shader.PropertyToID("_DensityNoise");
    private static int PropertyId_DensityNoiseSize = Shader.PropertyToID("_DensityNoiseSize");

    private static int PropertyId_AreaMin = Shader.PropertyToID("_AreaMin");
    private static int PropertyId_AreaMax = Shader.PropertyToID("_AreaMax");

    public static void Parametrize(this Material material, CloudConfig cloudConfig)
    {
        material.Parametrize(cloudConfig.CameraParameters);
        material.Parametrize(cloudConfig.LightParameters);
        material.Parametrize(cloudConfig.CylinderParameters);
        material.Parametrize(cloudConfig.ShapeParameters);
        material.Parametrize(cloudConfig.DensityParameters);
        material.Parametrize(cloudConfig.AreaParameters);
    }

    public static void Parametrize(this Material material, CameraParameters cameraParameters)
    {
        material.SetMatrix(PropertyId_CamToWorld, cameraParameters.CamToWorld);
        material.SetMatrix(PropertyId_CameraInverseProjection, cameraParameters.CamInverseProjection);
        material.SetVector(PropertyId_CamPos, cameraParameters.CamPos);
    }

    public static void Parametrize(this Material material, LightParameters lightParameters)
    {
        material.SetVector(PropertyId_LightColor, lightParameters.LightColor);
        material.SetVector(PropertyId_LightPosition, lightParameters.LightPosition);
    }

    public static void Parametrize(this Material material, CylinderParameters cylinderParameters)
    {
        material.SetVectorArray(PropertyId_Cylinders, cylinderParameters.Cylinders);
        material.SetMatrixArray(PropertyId_CylinderMatrices, new Matrix4x4[cylinderParameters.CylinderCount]);
        material.SetInt(PropertyId_CylinderCount, cylinderParameters.CylinderCount);
    }

    public static void Parametrize(this Material material, ShapeParameters shapeParameters)
    {
        material.SetFloat(PropertyId_RadiusMultiplier, shapeParameters.RadiusMultiplier);
        material.SetFloat(PropertyId_HeightMultiplier, shapeParameters.HeightMultiplier);
        material.SetFloat(PropertyId_RadiusThreshold, shapeParameters.RadiusThreshold);
    }

    public static void Parametrize(this Material material, DensityParameters densityParameters)
    {
        material.SetTexture(PropertyId_DensityNoise, densityParameters.DensityNoise);
        material.SetInt(PropertyId_DensityNoiseSize, densityParameters.DensityNoiseSize);
    }

    public static void Parametrize(this Material material, AreaParameters areaParameters)
    {
        material.SetVector(PropertyId_AreaMin, areaParameters.AreaMin);
        material.SetVector(PropertyId_AreaMax, areaParameters.AreaMax);
    }
}
