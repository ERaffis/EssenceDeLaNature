namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Sharpen")]
    public sealed class SharpenSettings : VolumeComponent, IPostProcessComponent
    {
        [Range(0f, 1f), Tooltip("Sharpen effect intensity.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        public bool IsActive()
        {
            return intensity.value > 0.0f && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
