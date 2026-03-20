using UnityEngine;

namespace Assets.Scripts.Models
{
    public class AreaParameters
    {
        public Vector3 AreaMin { get; set; }
        public Vector3 AreaMax { get; set; }
        public AreaParameters(Vector3 areaMin, Vector3 areaMax)
        {
            AreaMin = areaMin;
            AreaMax = areaMax;
        }
    }
}