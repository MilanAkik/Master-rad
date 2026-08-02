using Assets.Scripts.Models;
using System.Collections.Generic;
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

        [System.NonSerialized]
        private int maxLoops = 5;

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters)
        {
            var currentState = startingState;
            var matrices = getMatricesFromCurrentState(currentState, parameters);
            var numLoops = 0;
            while (matrices.Length < parameters.CylinderCount && numLoops < maxLoops)
            {
                currentState = applyAxiomsToState(currentState, parameters);
                matrices = getMatricesFromCurrentState(currentState, parameters);
                numLoops++;
            }
            return matrices;
        }

        private string applyAxiomsToState(string currentState, GeneratorParameters parameters)
        {
            var newState = "";
            foreach (var c in currentState)
            {
                switch (c)
                {
                    case 'F': newState += string.IsNullOrEmpty(axiomF) ? "F" : axiomF; break;
                    case 'B': newState += string.IsNullOrEmpty(axiomB) ? "B" : axiomB; break;
                    case 'L': newState += string.IsNullOrEmpty(axiomL) ? "L" : axiomL; break;
                    case 'R': newState += string.IsNullOrEmpty(axiomR) ? "R" : axiomR; break;
                    case 'U': newState += string.IsNullOrEmpty(axiomU) ? "U" : axiomU; break;
                    case 'D': newState += string.IsNullOrEmpty(axiomD) ? "D" : axiomD; break;
                    case 'G': newState += string.IsNullOrEmpty(axiomG) ? "G" : axiomG; break;
                    case 'S': newState += string.IsNullOrEmpty(axiomS) ? "S" : axiomS; break;
                    case 'P': newState += string.IsNullOrEmpty(axiomP) ? "P" : axiomP; break;
                    case 'V': newState += string.IsNullOrEmpty(axiomV) ? "V" : axiomV; break;
                    default: newState += c; break;
                }
            }
            return newState;
        }

        private Matrix4x4[] getMatricesFromCurrentState(string currentState, GeneratorParameters parameters)
        {
            var matricesList = new List<Matrix4x4>();
            var stateList = new List<LSystemState>();
            stateList.Add(new LSystemState(new Vector3(0.5f,0.5f,0.5f), 0f, 1f, 1f));
            foreach (var c in currentState)
            {
                switch (c)
                {
                    case 'F':
                        break;
                    case 'B':
                        break;
                    case 'L':
                        break;
                    case 'R':
                        break;
                    case 'U':
                        break;
                    case 'D':
                        break;
                    case 'G':
                        break;
                    case 'S':
                        break;
                    case 'P':
                        break;
                    case 'V':
                        break;
                    default:
                        break;
                }
            }
            return matricesList.ToArray();
        }

        private record LSystemState
        {
            public Vector3 position;
            public float angle;
            public float radius;
            public float height;

            public LSystemState(Vector3 position, float angle, float radius, float height)
            {
                this.position = position;
                this.angle = angle;
                this.radius = radius;
                this.height = height;
            }
        }
    }
}
