namespace SnapshotShaders
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Light Streaks")]
    public sealed class LightStreaksSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Light Streaks blur strength.")]
        public ClampedIntParameter strength = new ClampedIntParameter(1, 1, 500);

        [Tooltip("Luminance Threshold - pixels above this luminance will glow.")]
        public ClampedFloatParameter luminanceThreshold = new ClampedFloatParameter(10.0f, 0.0f, 25.0f);

        public bool IsActive()
        {
            return strength.value > 1 && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
