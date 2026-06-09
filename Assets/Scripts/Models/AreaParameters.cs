using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class AreaParameters
    {
        [SerializeField] private Vector3 areaMin = new Vector3(-10, -10, -10);
        [SerializeField] private Vector3 areaMax = new Vector3(10, 10, 10);
        public Vector3 AreaMin => areaMin;
        public Vector3 AreaMax => areaMax;

        public AreaParameters(Vector3 areaMin, Vector3 areaMax)
        {
            this.areaMin = areaMin;
            this.areaMax = areaMax;
        }
    }
}