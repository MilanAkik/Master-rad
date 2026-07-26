using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class GeneratorParameters
    {
        [SerializeField] private int cylinderCount;
        [SerializeField] private int randomSeed;
        [SerializeField] private float radiusMultiplier;
        [SerializeField] private float heightMultiplier;

        public int CylinderCount { get => cylinderCount; set => cylinderCount = value; }
        public int RandomSeed { get => randomSeed; set => randomSeed = value; }
        public float RadiusMultiplier { get => radiusMultiplier; set => radiusMultiplier = value; }
        public float HeightMultiplier { get => heightMultiplier; set => heightMultiplier = value; }
    }
}
