using UnityEngine;

namespace Assets.Scripts.Generators
{
    internal interface ICylinderGenerator
    {
        public abstract Vector4[] getCylinders(params object[] objects);
    }
}
