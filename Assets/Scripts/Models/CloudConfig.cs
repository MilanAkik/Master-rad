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

        public AreaParameters AreaParameters => areaParameters;
        public CameraParameters CameraParameters { get => cameraParameters; set => cameraParameters = value; }
        public CylinderParameters CylinderParameters => cylinderParameters;
        public DensityParameters DensityParameters => densityParameters;
        public LightParameters LightParameters => lightParameters;

        public CloudConfig(AreaParameters areaParameters, CameraParameters cameraParameters, CylinderParameters cylinderParameters, DensityParameters densityParameters, LightParameters lightParameters)
        {
            this.areaParameters = areaParameters;
            this.cameraParameters = cameraParameters;
            this.cylinderParameters = cylinderParameters;
            this.densityParameters = densityParameters;
            this.lightParameters = lightParameters;
        }

    }
}
