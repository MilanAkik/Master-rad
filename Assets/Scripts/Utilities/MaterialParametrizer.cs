using Assets.Scripts.Models;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public static class MaterialParametrizer
{
    private static int PropertyId_CamToWorld = Shader.PropertyToID("_CamToWorld");
    private static int PropertyId_CameraInverseProjection = Shader.PropertyToID("_CamInverseProjection");
    private static int PropertyId_CamPos = Shader.PropertyToID("_CamPos");

    private static int PropertyId_LightColor = Shader.PropertyToID("_LightColor");
    private static int PropertyId_LightPosition = Shader.PropertyToID("_LightPosition");

    private static int PropertyId_CylinderMatrices = Shader.PropertyToID("_CylinderMatrices");
    private static int PropertyId_CylinderCount = Shader.PropertyToID("_CylinderCount");

    private static int PropertyId_DensityNoise = Shader.PropertyToID("_DensityNoise");
    private static int PropertyId_DensityNoiseSize = Shader.PropertyToID("_DensityNoiseSize");

    private static int PropertyId_AreaMin = Shader.PropertyToID("_AreaMin");
    private static int PropertyId_AreaMax = Shader.PropertyToID("_AreaMax");

    public static void Parametrize(this Material material, CloudConfig cloudConfig)
    {
        material.Parametrize(cloudConfig.CameraParameters);
        material.Parametrize(cloudConfig.LightParameters);
        material.Parametrize(cloudConfig.CylinderParameters);
        material.Parametrize(cloudConfig.DensityParameters);
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
        var cylinderMatrices = cylinderParameters.CylinderMatrices;
        if (cylinderMatrices.Length > 0)
        {
            material.SetMatrixArray(PropertyId_CylinderMatrices, cylinderMatrices.Select(x=>x.transpose).ToList());
        }
        material.SetInt(PropertyId_CylinderCount, cylinderMatrices.Length);
        material.SetVector(PropertyId_AreaMin, cylinderParameters.GeneratorParameters.AreaMin);
        material.SetVector(PropertyId_AreaMax, cylinderParameters.GeneratorParameters.AreaMax);
    }

    public static void Parametrize(this Material material, DensityParameters densityParameters)
    {
        material.SetTexture(PropertyId_DensityNoise, densityParameters.DensityNoise);
        material.SetInt(PropertyId_DensityNoiseSize, densityParameters.DensityNoiseSize);
    }
}
