using Assets.Scripts.Generators;
using UnityEngine;


namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class CylinderParameters
    {
        [SerializeField] private GeneratorParameters generatorParameters;
        [SerializeField] private CylinderGenerator generator;
        [SerializeField] private Vector4[] cylinders;
        [SerializeField] private int cylinderCount;

        public GeneratorParameters GeneratorParameters => generatorParameters;
        public CylinderGenerator Generator => generator;
        public Vector4[] Cylinders { get{ return cylinders; } set { cylinders = value; } }
        public int CylinderCount => cylinders.Length;

        public CylinderParameters(Vector4[] cylinders, int cylinderCount)
        {
            this.cylinders = cylinders;
            this.cylinderCount = cylinderCount;
        }
    }
}