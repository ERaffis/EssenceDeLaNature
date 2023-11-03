namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/RadialBlur")]
    public sealed class RadialBlurSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Blur Strength")]
        public ClampedIntParameter strength = new ClampedIntParameter(1, 1, 500);

        [Range(0.0f, 1.0f), Tooltip("Proportion of the screen which is unblurred.")]
        public ClampedFloatParameter focalSize = new ClampedFloatParameter(0.25f, 0.0f, 1.0f);

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
