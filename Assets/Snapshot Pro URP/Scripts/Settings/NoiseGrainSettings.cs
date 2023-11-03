namespace SnapshotShaders.URP
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [Serializable, VolumeComponentMenu("Snapshot Shaders Pro/Noise Grain")]
    public sealed class NoiseGrainSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("How strongly the screen colors get lightened by noise.")]
        public FloatParameter strength = new FloatParameter(0.0f);

        [Tooltip("How fast the noise grain changes values.")]
        public FloatParameter speed = new FloatParameter(1.0f);

        [Tooltip("The size of the noise texture that gets applied to the screen.")]
        public FloatParameter noiseSize = new FloatParameter(1.0f);

        [Tooltip("Hermite interpolation is faster, while Quintic interpolation will look very slightly nicer.")]
        public NoiseInterpParameter noiseInterpolation = new NoiseInterpParameter(NoiseInterpolation.Quintic);

        public bool IsActive()
        {
            return strength.value > 0.0f && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

    [Serializable]
    public enum NoiseInterpolation
    {
        Hermite, Quintic
    }

    [Serializable]
    public sealed class NoiseInterpParameter : VolumeParameter<NoiseInterpolation>
    {
        public NoiseInterpParameter(NoiseInterpolation value, bool overrideState = false) : base(value, overrideState) { }
    }
}
