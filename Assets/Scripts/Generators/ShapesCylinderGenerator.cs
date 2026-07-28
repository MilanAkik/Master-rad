using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Shapes")]
    public class ShapesCylinderGenerator : CylinderGenerator
    {

        public enum ShapeType { Cube, Circle, Sphere }

        public ShapeType shapeType = ShapeType.Cube;

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];
    }
}
