namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Cutout")]
    public sealed class CutoutSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Is the effect active?")]
        public BoolParameter enabled = new BoolParameter(false);

        [Tooltip("The texture to use for the cutout.")]
        public TextureParameter cutoutTexture = new TextureParameter(null);

        [Tooltip("The colour of the area outside the cutout.")]
        public ColorParameter borderColor = new ColorParameter(Color.white);

        [Tooltip("Should the cutout texture stretch to fit the screen's aspect ratio?")]
        public BoolParameter stretch = new BoolParameter(false);

        [Tooltip("How zoomed-in the texture is. 1 = unzoomed.")]
        public ClampedFloatParameter zoom = new ClampedFloatParameter(1.0f, 0.01f, 10.0f);

        [Tooltip("How offset the texture is from the centre of the screen (in UV space).")]
        public Vector2Parameter offset = new Vector2Parameter(Vector2.zero);

        [Range(0.0f, 360.0f), Tooltip("How much the texture is rotated (anticlockwise, in degrees).")]
        public ClampedFloatParameter rotation = new ClampedFloatParameter(0.0f, 0.0f, 360.0f);

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
