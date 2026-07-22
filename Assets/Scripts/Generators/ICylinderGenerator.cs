using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    internal interface ICylinderGenerator
    {
        public abstract Vector4[] getCylinders(GeneratorParameters parameters);
        public abstract Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters);
    }
}
