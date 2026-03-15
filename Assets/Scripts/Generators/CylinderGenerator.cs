using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    public abstract class CylinderGenerator : ScriptableObject, ICylinderGenerator
    {
        public abstract Vector4[] getCylinders(GeneratorParameters parameters);
    }
}
