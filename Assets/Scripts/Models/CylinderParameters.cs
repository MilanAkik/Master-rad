using UnityEngine;


namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class CylinderParameters
    {
        [SerializeField] private Vector4[] cylinders;
        [SerializeField] private int cylinderCount;
        public Vector4[] Cylinders => cylinders;
        public int CylinderCount => cylinderCount;

        public CylinderParameters(Vector4[] cylinders, int cylinderCount)
        {
            this.cylinders = cylinders;
            this.cylinderCount = cylinderCount;
        }
    }
}