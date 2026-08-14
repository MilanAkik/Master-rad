using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Shapes")]
    public class ShapesCylinderGenerator : CylinderGenerator
    {

        public enum ShapeType { Cube, Circle, Sphere, Spiral }

        public ShapeType shapeType = ShapeType.Cube;
        public int spiralTurns = 3;

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters) => shapeType switch
        {
            ShapeType.Cube => GenerateCubeCylinders(parameters),
            ShapeType.Circle => GenerateCircleCylinders(parameters),
            ShapeType.Sphere => GenerateSphereCylinders(parameters),
            ShapeType.Spiral => GenerateSpiralCylinders(parameters),
            _ => new Matrix4x4[parameters.CylinderCount]
        };

        private Matrix4x4[] GenerateCubeCylinders(GeneratorParameters parameters)
        {
            var count = parameters.CylinderCount;
            var matrices = new Matrix4x4[count];
            int i = 0;
            for (; i * i * i < count; i++);
            float size = (float)i;
            for (int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    for(int z = 0; z < size; z++)
                    {
                        int index = (int)(x * size * size + y * size + z);
                        if (index < parameters.CylinderCount)
                        {
                            float posX = Mathf.Lerp(parameters.AreaMin.x, parameters.AreaMax.x, x / (size - 1));
                            float posY = Mathf.Lerp(parameters.AreaMin.y, parameters.AreaMax.y, y / (size - 1));
                            float posZ = Mathf.Lerp(parameters.AreaMin.z, parameters.AreaMax.z, z / (size - 1));
                            float h = (1 / size) * parameters.HeightMultiplier;
                            float r = (1 / size) * parameters.RadiusMultiplier;
                            matrices[index] = new Matrix4x4(new Vector4(posX, posY, posZ, r), new Vector4(h, 0, 0, 0), Vector4.zero, Vector4.zero);
                        }
                    }
                }
            }
            return matrices;
        }

        private Matrix4x4[] GenerateCircleCylinders(GeneratorParameters parameters)
        {
            var count = parameters.CylinderCount;
            var step = 360.0f / count;
            var matrices = new Matrix4x4[count];
            for (int i = 0; i < count; i++)
            {
                var angle = i * step * Mathf.Deg2Rad;
                var x = 0.5f + 0.5f * Mathf.Cos(angle);
                var y = 0.5f;
                var z = 0.5f + 0.5f * Mathf.Sin(angle);
                var r = 1f;
                var h = 1f;
                x = Mathf.Lerp(parameters.AreaMin.x, parameters.AreaMax.x, x);
                y = Mathf.Lerp(parameters.AreaMin.y, parameters.AreaMax.y, y);
                z = Mathf.Lerp(parameters.AreaMin.z, parameters.AreaMax.z, z);
                r = r * parameters.RadiusMultiplier;
                h = h * parameters.HeightMultiplier;
                matrices[i] = new Matrix4x4(new Vector4(x, y, z, r), new Vector4(h, 0, 0, 0), Vector4.zero, Vector4.zero);
            }
            return matrices;
        }
        
        private Matrix4x4[] GenerateSphereCylinders(GeneratorParameters parameters)
        {
            var count = parameters.CylinderCount;
            var step = 1.0f / count;
            var matrices = new Matrix4x4[count];
            for(int i = 0; i < count; i++)
            {
                var x = 0.5f;
                var y = i * step;
                var z = 0.5f;
                var r = Mathf.Abs((2 * i + 1 - count) * step);
                var h = step * (parameters.AreaMax.y - parameters.AreaMin.y);
                var xSize = 0.5f * (parameters.AreaMax.x - parameters.AreaMin.x);
                var zSize = 0.5f * (parameters.AreaMax.z - parameters.AreaMin.z);
                var rSize = (xSize < zSize ? xSize : zSize);
                x = Mathf.Lerp(parameters.AreaMin.x, parameters.AreaMax.x, x);
                y = Mathf.Lerp(parameters.AreaMin.y, parameters.AreaMax.y, y);
                z = Mathf.Lerp(parameters.AreaMin.z, parameters.AreaMax.z, z);
                r = Mathf.Sqrt(1 - (r * r)) * rSize;
                matrices[i] = new Matrix4x4(new Vector4(x, y, z, r), new Vector4(h, 0, 0, 0), Vector4.zero, Vector4.zero);
            }
            return matrices;
        }

        private Matrix4x4[] GenerateSpiralCylinders(GeneratorParameters parameters)
        {
            var count = parameters.CylinderCount;
            var step = (spiralTurns * 360.0f) / count;
            var matrices = new Matrix4x4[count];
            for (int i = 0; i < count; i++)
            {
                var angle = i * step * Mathf.Deg2Rad;
                var t = (i+1f) / (count);
                var distMod = Mathf.Pow(t, 1.2f);
                var radMod = Mathf.Pow(t, 1.6f);
                var x = 0.5f + 0.5f * Mathf.Cos(angle) * distMod;
                var y = 0.5f;
                var z = 0.5f + 0.5f * Mathf.Sin(angle) * distMod;
                var r = radMod;
                var h = radMod;
                x = Mathf.Lerp(parameters.AreaMin.x, parameters.AreaMax.x, x);
                y = Mathf.Lerp(parameters.AreaMin.y, parameters.AreaMax.y, y);
                z = Mathf.Lerp(parameters.AreaMin.z, parameters.AreaMax.z, z);
                r = Mathf.Lerp(0.3f, 1f,r) * parameters.RadiusMultiplier;
                h = h * parameters.HeightMultiplier;
                matrices[i] = new Matrix4x4(new Vector4(x, y, z, r), new Vector4(h, 0, 0, 0), Vector4.zero, Vector4.zero);
            }
            return matrices;
        }

    }
}
