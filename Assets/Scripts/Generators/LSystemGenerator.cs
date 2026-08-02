using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    [CreateAssetMenu(menuName = "CylinderGenerator/LSystem")]
    public class LSystemGenerator : CylinderGenerator
    {
        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters) => new Matrix4x4[parameters.CylinderCount];
    }
}
