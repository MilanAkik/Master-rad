using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    [CreateAssetMenu(menuName = "CylinderGenerator/LSystem")]
    public class LSystemGenerator : CylinderGenerator
    {
        // Foreward, Backward, Left, Right, Up, Down, Grow, Shrink, Place, VSplit
        public string axiomF = "";
        public string axiomB = "";
        public string axiomL = "";
        public string axiomR = "";
        public string axiomU = "";
        public string axiomD = "";
        public string axiomG = "";
        public string axiomS = "";
        public string axiomP = "";
        public string axiomV = "";
        public string startingState = "";

        private int maxLoops = 100;

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters)
        {
            string currentState = startingState;
            var matrices = new Matrix4x4[parameters.CylinderCount];
            //Matrix4x4[] matrices = getMatricesFromCurrentState(currentState, parameters);
            var numLoops = 0;
            while (matrices.Length < parameters.CylinderCount && numLoops < maxLoops)
            {
                //currentState = applyAxiomsToState(currentState, parameters);
                //matrices = getMatricesFromCurrentState(currentState, parameters);
                numLoops++;
            }
            return matrices;
        }
    }
}
