using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class GeneratorParameters
    {
        [SerializeField] private int cylinderCount;
        [SerializeField] private int randomSeed;

        public int CylinderCount { get => cylinderCount; set => cylinderCount = value; }
        public int RandomSeed { get => randomSeed; set => randomSeed = value; }
    }
}
