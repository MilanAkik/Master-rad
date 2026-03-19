using UnityEngine;

public class CylinderParameters
{
    public Vector4[] Cylinders { get; set; }
    public int CylinderCount { get; set; }
    public CylinderParameters(Vector4[] cylinders, int cylinderCount)
    {
        Cylinders = cylinders;
        CylinderCount = cylinderCount;
    }
}
