using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Shapes")]
    public class ShapesCylinderGenerator : CylinderGenerator
    {

        public enum ShapeType { Cube, Circle, Sphere }

        public ShapeType shapeType = ShapeType.Cube;

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters) => shapeType switch
        {
            ShapeType.Cube => GenerateCubeCylinders(parameters),
            ShapeType.Circle => GenerateCircleCylinders(parameters),
            ShapeType.Sphere => GenerateSphereCylinders(parameters),
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

        private Matrix4x4[] GenerateCircleCylinders(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];
        private Matrix4x4[] GenerateSphereCylinders(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];

    }
}
