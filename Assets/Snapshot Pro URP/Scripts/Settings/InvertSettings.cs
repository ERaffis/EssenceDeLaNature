namespace SnapshotShaders
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Invert")]
    public sealed class InvertSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Invert effect intensity.")]
        public ClampedFloatParameter strength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        public bool IsActive()
        {
            return strength.value > 0.0f && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
