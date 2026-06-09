using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ShapeParameters
    {
        [SerializeField] private float radiusMultiplier = 1f;
        [SerializeField] private float heightMultiplier = 1f;
        [SerializeField] private float radiusThreshold = 0.5f;
        public float RadiusMultiplier => radiusMultiplier;
        public float HeightMultiplier => heightMultiplier;
        public float RadiusThreshold => radiusThreshold;

        public ShapeParameters(float radiusMultiplier, float heightMultiplier, float radiusThreshold)
        {
            this.radiusMultiplier = radiusMultiplier;
            this.heightMultiplier = heightMultiplier;
            this.radiusThreshold = radiusThreshold;
        }
    }
}