using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Random")]
    public class RandomCylinderGenerator : CylinderGenerator
    {
        public override Vector4[] getCylinders(params object[] objects)
        {
            int count = (int)objects[0];
            int seed = (int)objects[1];
            var prevState = Random.state;
            Random.InitState(seed);
            Vector4[] res = new Vector4[count];
            for (int i = 0; i < count; i++)
            {
                var x = Random.Range(-1.0f, 1.0f);
                var y = Random.Range(-0.5f, 0.5f);
                var r = Random.Range(0.01f, 0.99f);
                var z = Random.Range(-1.0f, 1.0f);
                res[i] = new Vector4(x, y, z, r);
            }
            Random.state = prevState;
            return res;
        }
    }
}
