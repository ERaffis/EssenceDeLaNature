namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class FancyNeon : ScriptableRendererFeature
    {
        class FancyNeonRenderPass : ScriptableRenderPass
        {
            private Material material;
            private FancyNeonSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            public FancyNeonRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<FancyNeonSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Neon"));
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (settings == null)
                {
                    return;
                }

                mainTex = new RenderTargetHandle();
                mainTex.id = Shader.PropertyToID("MainTex");
                cmd.GetTemporaryRT(mainTex.id, cameraTextureDescriptor);

                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

                // Copy camera color texture to MainTex.
                cmd.Blit(source, mainTex.id);

                // Set Fancy Neon effect properties.
                if (settings.useSceneColor.value)
                {
                    material.EnableKeyword("USE_SCENE_TEXTURE_ON");
                }
                else
                {
                    material.DisableKeyword("USE_SCENE_TEXTURE_ON");
                    cmd.SetGlobalColor("_BackgroundColor", settings.backgroundColor.value);
                }

                cmd.SetGlobalFloat("_ColorSensitivity", settings.colorSensitivity.value);
                cmd.SetGlobalFloat("_ColorStrength", settings.colorStrength.value);
                cmd.SetGlobalFloat("_DepthSensitivity", settings.depthSensitivity.value);
                cmd.SetGlobalFloat("_DepthStrength", settings.depthStrength.value);
                cmd.SetGlobalFloat("_NormalsSensitivity", settings.normalSensitivity.value);
                cmd.SetGlobalFloat("_NormalsStrength", settings.normalStrength.value);
                cmd.SetGlobalFloat("_DepthThreshold", settings.depthThreshold.value);
                cmd.SetGlobalFloat("_SaturationFloor", settings.saturationFloor.value);
                cmd.SetGlobalFloat("_LightnessFloor", settings.lightnessFloor.value);
                cmd.SetGlobalColor("_EmissiveColor", settings.emissiveEdgeColor.value);

                // Execute effect using effect material.
                cmd.Blit(mainTex.id, source, material);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(mainTex.id);
            }
        }
        
        FancyNeonRenderPass pass;

        public override void Create()
        {
            pass = new FancyNeonRenderPass("Fancy Neon");
            name = "Fancy Neon";
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            pass.Setup(renderer);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            pass.EnqueuePass(renderer);
        }
    }
}
