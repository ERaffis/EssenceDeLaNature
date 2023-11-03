namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Halftone : ScriptableRendererFeature
    {
        class HalftoneRenderPass : ScriptableRenderPass
        {
            private Material material;
            private HalftoneSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            public HalftoneRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<HalftoneSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Halftone"));
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
                if (!settings.IsActive())
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

                // Copy camera color texture to MainTex.
                cmd.Blit(source, mainTex.id);

                // Set Halftone effect properties.
                if (settings.useSceneColor.value)
                {
                    cmd.EnableShaderKeyword("USE_SCENE_TEXTURE_ON");
                }
                else
                {
                    cmd.DisableShaderKeyword("USE_SCENE_TEXTURE_ON");
                }

                cmd.SetGlobalTexture("_HalftoneTexture", settings.halftoneTexture.value);
                cmd.SetGlobalFloat("_Softness", settings.softness.value);
                cmd.SetGlobalFloat("_TextureSize", settings.textureSize.value);
                cmd.SetGlobalVector("_MinMaxLuminance", settings.minMaxLuminance.value);
                cmd.SetGlobalColor("_DarkColor", settings.darkColor.value);
                cmd.SetGlobalColor("_LightColor", settings.lightColor.value);

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

        HalftoneRenderPass pass;

        public override void Create()
        {
            pass = new HalftoneRenderPass("Halftone");
            name = "Halftone";
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
