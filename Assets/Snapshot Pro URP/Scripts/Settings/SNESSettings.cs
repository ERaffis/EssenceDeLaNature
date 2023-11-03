namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/SNES")]
    public sealed class SNESSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Is the effect active?")]
        public BoolParameter enabled = new BoolParameter(false);

        [Tooltip("Strength/size of the waves.")]
        public ClampedIntParameter bandingValues = new ClampedIntParameter(6, 1, 16);

        [Tooltip("Modify the input colors via a power ramp. 1 = original mapping, " +
            "higher = favors darker output, lower = favors lighter output.")]
        public ClampedFloatParameter powerRamp = new ClampedFloatParameter(1.0f, 0.0f, 4.0f);

        public bool IsActive()
        {
            return enabled.value && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
