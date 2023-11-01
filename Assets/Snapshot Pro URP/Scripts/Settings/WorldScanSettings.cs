namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/World Scan")]
    public sealed class WorldScanSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("The world space origin point of the scan.")]
        public Vector3Parameter scanOrigin = new Vector3Parameter(Vector3.zero);

        //[Tooltip("How quickly, in units per second, the scan propogates.")]
        //public FloatParameter scanSpeed = new FloatParameter(1.0f);

        [Tooltip("How far, in Unity units, the scan has travelled from the origin.")]
        public FloatParameter scanDistance = new FloatParameter(0.0f);

        [Tooltip("The distance, in Unity units, the scan texture gets applied over.")]
        public FloatParameter scanWidth = new FloatParameter(1.0f);

        [Tooltip("An x-by-1 ramp texture representing the scan color.")]
        public TextureParameter overlayRampTex = new TextureParameter(null);
        
        [Tooltip("An additional HDR color tint applied to the scan.")]
        public ColorParameter overlayColor = new ColorParameter(Color.white, true, true, true);

        public bool IsActive()
        {
            return overlayRampTex.value != null && active;
        }

        public bool IsTileCompatible()
        {
            return true;
        }
    }
}
