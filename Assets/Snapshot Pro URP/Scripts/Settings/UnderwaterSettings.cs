namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Underwater")]
    public class UnderwaterSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Displacement texture for surface waves.")]
        public TextureParameter bumpMap = new TextureParameter(null);

        [Range(0.0f, 10.0f), Tooltip("Strength/size of the waves.")]
        public ClampedFloatParameter strength = new ClampedFloatParameter(0.0f, 0.0f, 10.0f);

        [Tooltip("Tint of the underwater fog.")]
        public ColorParameter waterFogColor = new ColorParameter(Color.white);

        [Range(0.0f, 1.0f), Tooltip("Strength of the underwater fog.")]
        public ClampedFloatParameter fogStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        public bool IsActive()
        {
            return (strength.value > 0.0f || fogStrength.value > 0.0f) && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
