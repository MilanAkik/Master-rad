using Assets.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Random")]
    public class RandomCylinderGenerator : CylinderGenerator
    {
        public override Vector4[] getCylinders(GeneratorParameters parameters)
        {
            int count = parameters.CylinderCount;
            int seed = parameters.RandomSeed;
            var prevState = Random.state;
            Random.InitState(seed);
            Vector4[] res = new Vector4[count];
            for (int i = 0; i < count; i++)
            {
                var x = Random.Range(-1.0f, 1.0f);
                var y = Random.Range(-0.5f, 0.5f);
                var r = Random.Range(0.01f, 0.99f);
                var z = Random.Range(-1.0f, 1.0f);
                res[i] = new Vector4(x, y, z+5.0f, r);
            }
            Random.state = prevState;
            return res;
        }

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters)
        {
            int count = parameters.CylinderCount;
            int seed = parameters.RandomSeed;
            var res = new Matrix4x4[count];
            var prevState = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < count; i++)
            {
                var x = Random.Range(-1.0f, 1.0f);
                var y = Random.Range(-0.5f, 0.5f);
                var r = Random.Range(0.01f, 0.99f);
                var z = Random.Range(-1.0f, 1.0f);
                res[i] = new Matrix4x4(
                    new Vector4(x, y, z + 5.0f, r),
                    new Vector4(1,0,0,0),
                    Vector4.zero,
                    Vector4.zero
                );
            }
            Random.state = prevState;
            return res;
        }
    }
}
