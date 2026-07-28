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

        private Matrix4x4[] GenerateCubeCylinders(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];
        private Matrix4x4[] GenerateCircleCylinders(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];
        private Matrix4x4[] GenerateSphereCylinders(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];

    }
}
