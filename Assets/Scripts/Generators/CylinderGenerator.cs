using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    public abstract class CylinderGenerator : ScriptableObject, ICylinderGenerator
    {
        public abstract Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters);
    }
}
