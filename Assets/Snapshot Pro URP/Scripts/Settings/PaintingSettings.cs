namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Painting")]
    public sealed class PaintingSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Oil Painting effect radius.")]
        public ClampedIntParameter kernelSize = new ClampedIntParameter(1, 1, 51);

        public bool IsActive()
        {
            return kernelSize.value > 1 && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
