using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class CloudConfig
    {

        [SerializeField] private AreaParameters areaParameters;
        [SerializeField] private CameraParameters cameraParameters;
        [SerializeField] private CylinderParameters cylinderParameters;
        [SerializeField] private DensityParameters densityParameters;
        [SerializeField] private LightParameters lightParameters;
        [SerializeField] private ShapeParameters shapeParameters;

        public AreaParameters AreaParameters => areaParameters;
        public CameraParameters CameraParameters { get => cameraParameters; set => cameraParameters = value; }
        public CylinderParameters CylinderParameters => cylinderParameters;
        public DensityParameters DensityParameters => densityParameters;
        public LightParameters LightParameters => lightParameters;
        public ShapeParameters ShapeParameters => shapeParameters;

        public CloudConfig(AreaParameters areaParameters, CameraParameters cameraParameters, CylinderParameters cylinderParameters, DensityParameters densityParameters, LightParameters lightParameters, ShapeParameters shapeParameters)
        {
            this.areaParameters = areaParameters;
            this.cameraParameters = cameraParameters;
            this.cylinderParameters = cylinderParameters;
            this.densityParameters = densityParameters;
            this.lightParameters = lightParameters;
            this.shapeParameters = shapeParameters;
        }

    }
}
