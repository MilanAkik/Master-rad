using UnityEngine;

namespace Assets.Scripts.Generators
{
    public abstract class CylinderGenerator
    {
        public abstract Vector4[] getCylinders(params object[] objects);
    }
}
