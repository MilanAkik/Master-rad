using Assets.Scripts.Generators;
using UnityEngine;


namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class CylinderParameters
    {
        [SerializeField] private GeneratorParameters generatorParameters;
        [SerializeField] private CylinderGenerator generator;

        public GeneratorParameters GeneratorParameters => generatorParameters;
        public CylinderGenerator Generator => generator;
        public Matrix4x4[] CylinderMatrices => generator.getCylinderMatrices(generatorParameters);
        public int CylinderCount => generatorParameters.CylinderCount;

        public CylinderParameters(GeneratorParameters generatorParameters, CylinderGenerator generator)
        {
            this.generatorParameters = generatorParameters;
            this.generator = generator;
        }
    }
}