using Assets.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Generators
{

    [CreateAssetMenu(menuName = "CylinderGenerator/Random")]
    public class RandomCylinderGenerator : CylinderGenerator
    {

        private Vector2 _xRange = new Vector2(0f, 1.0f);
        private Vector2 _yRange = new Vector2(0f, 1.0f);
        private Vector2 _zRange = new Vector2(0f, 1.0f);
        private Vector2 _rRange = new Vector2(0.01f, 0.99f);
        private Vector2 _hRange = new Vector2(0.01f, 0.99f);

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters)
        {
            int count = parameters.CylinderCount;
            int seed = parameters.RandomSeed;
            Vector3 areaMin = parameters.AreaMin;
            Vector3 areaMax = parameters.AreaMax;
            Vector3 areaSize = areaMax - areaMin;
            var res = new Matrix4x4[count];
            var prevState = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < count; i++)
            {
                var x = areaMin.x + Random.Range(_xRange.x, _xRange.y) * areaSize.x;
                var y = areaMin.y + Random.Range(_yRange.x, _yRange.y) * areaSize.y;
                var z = areaMin.z + Random.Range(_zRange.x, _zRange.y) * areaSize.z;
                Debug.Log($"RandomCylinderGenerator: x={x}, y={y}, z={z}");
                var r = Random.Range(_rRange.x, _rRange.y) * parameters.RadiusMultiplier;
                var h = Random.Range(_hRange.x, _hRange.y) * parameters.HeightMultiplier;
                res[i] = new Matrix4x4( new Vector4(x, y, z, r), new Vector4(h,0,0,0), Vector4.zero, Vector4.zero );
            }
            Random.state = prevState;
            return res;
        }
        
    }
}
