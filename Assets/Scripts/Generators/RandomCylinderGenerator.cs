using Assets.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Random")]
    public class RandomCylinderGenerator : CylinderGenerator
    {

        private const float _RadiusMultiplier = 1.0f;
        private const float _RadiusThreshold = 0.2f;
        private const float _HeightMultiplier = 1.0f;

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
                var z = Random.Range(-1.0f, 1.0f);
                var r = Random.Range(0.01f, 0.99f);
                var h = getHeight(r);
                res[i] = new Matrix4x4(
                    new Vector4(x, y, z + 5.0f, r * _RadiusMultiplier),
                    new Vector4(h,0,0,0),
                    Vector4.zero,
                    Vector4.zero
                );
            }
            Random.state = prevState;
            return res;
        }

        private float getHeight(float radius)
        {
            float r1 = radius;
            if (r1 < _RadiusThreshold) return 0;
            float a1 = 2 / (_RadiusThreshold - 1);
            float a = a1 * a1;
            float b = -a * (_RadiusThreshold + 1);
            float c = 1 - a - b;
            float h = 2.0f - a * r1 * r1 - b * r1 - c;
            return _HeightMultiplier * h;
        }
    }
}
