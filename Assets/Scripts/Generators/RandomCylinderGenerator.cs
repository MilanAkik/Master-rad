using Assets.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Random")]
    public class RandomCylinderGenerator : CylinderGenerator
    {

        private Vector2 _xRange = new Vector2(-1.0f, 1.0f);
        private Vector2 _yRange = new Vector2(-0.1f, 0.1f);
        private Vector2 _zRange = new Vector2(-1.0f, 1.0f);
        private Vector2 _rRange = new Vector2(0.01f, 0.99f);
        private Vector2 _hRange = new Vector2(0.01f, 0.99f);

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
                var x = Random.Range(_xRange.x, _xRange.y);
                var y = Random.Range(_yRange.x, _yRange.y);
                var z = Random.Range(_zRange.x, _zRange.y) + 5.0f;
                var r = Random.Range(_rRange.x, _rRange.y) * parameters.RadiusMultiplier;
                var h = Random.Range(_hRange.x, _hRange.y) * parameters.HeightMultiplier;
                res[i] = new Matrix4x4(
                    new Vector4(x, y, z, r),
                    new Vector4(h,0,0,0),
                    Vector4.zero,
                    Vector4.zero
                );
            }
            Random.state = prevState;
            return res;
        }
        
    }
}
