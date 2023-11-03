namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Fancy Dither")]
    public sealed class FancyDitherSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Is the effect active?")]
        public BoolParameter enabled = new BoolParameter(false);

        [Tooltip("Noise texture to use for dither thresholding.")]
        public TextureParameter noiseTex = new TextureParameter(null);

        [Range(0.1f, 100.0f), Tooltip("Size of the noise texture.")]
        public ClampedFloatParameter noiseSize = new ClampedFloatParameter(1.0f, 0.1f, 100.0f);

        [Tooltip("Offset used when calculating luminance threshold.")]
        public ClampedFloatParameter thresholdOffset = new ClampedFloatParameter(0.0f, -0.5f, 0.5f);

        [Tooltip("Amount of blending between the three cardinal directions.")]
        public ClampedFloatParameter blendAmount = new ClampedFloatParameter(1.0f, 0.0f, 1.0f);

        [Tooltip("Color to use for dark sections of the image.")]
        public ColorParameter darkColor = new ColorParameter(Color.black);

        [Tooltip("Color to use for light sections of the image.")]
        public ColorParameter lightColor = new ColorParameter(Color.white);

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
