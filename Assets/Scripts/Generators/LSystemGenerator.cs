using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
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
        [System.NonSerialized]
        private float stepSize = 2f;
        [System.NonSerialized]
        private float rotationAngle = 45f;
        [System.NonSerialized]
        private Vector3 verticalMovement = new Vector3(0f, 0.1f, 0f);
        [System.NonSerialized]
        private float radiusMultiplier = 1.1f;

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
                        stateList = stateList.Select(s => new LSystemState(s.position + stepSize * new Vector3(Mathf.Sin(s.angle * Mathf.Deg2Rad), 0f, Mathf.Cos(s.angle * Mathf.Deg2Rad)), s.angle, s.radius, s.height)).ToList();
                        break;
                    case 'B':
                        stateList = stateList.Select(s => new LSystemState(s.position - stepSize * new Vector3(Mathf.Sin(s.angle * Mathf.Deg2Rad), 0f, Mathf.Cos(s.angle * Mathf.Deg2Rad)), s.angle, s.radius, s.height)).ToList();
                        break;
                    case 'L':
                        stateList = stateList.Select(s => new LSystemState(s.position, s.angle + rotationAngle, s.radius, s.height)).ToList();
                        break;
                    case 'R':
                        stateList = stateList.Select(s => new LSystemState(s.position, s.angle - rotationAngle, s.radius, s.height)).ToList();
                        break;
                    case 'U':
                        stateList = stateList.Select(s => new LSystemState(s.position + verticalMovement, s.angle, s.radius, s.height)).ToList();
                        break;
                    case 'D':
                        stateList = stateList.Select(s => new LSystemState(s.position - verticalMovement, s.angle, s.radius, s.height)).ToList();
                        break;
                    case 'G':
                        stateList = stateList.Select(s => new LSystemState(s.position, s.angle, s.radius * radiusMultiplier, s.height)).ToList();
                        break;
                    case 'S':
                        stateList = stateList.Select(s => new LSystemState(s.position, s.angle, s.radius / radiusMultiplier, s.height)).ToList();
                        break;
                    case 'P':
                        matricesList.AddRange(stateList.Select(s => new Matrix4x4(new Vector4(s.position.x, s.position.y, s.position.z, s.radius), new Vector4(s.height,0,0,0), new Vector4(0,0,0,0), new Vector4(0,0,0,0))));
                        break;
                    case 'V':
                        stateList = stateList.SelectMany(s => new List<LSystemState> {
                            new LSystemState(s.position, s.angle + rotationAngle, s.radius, s.height),
                            new LSystemState(s.position, s.angle - rotationAngle, s.radius, s.height)
                        }).ToList();    
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
