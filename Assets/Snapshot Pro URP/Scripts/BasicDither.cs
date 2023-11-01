namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class BasicDither : ScriptableRendererFeature
    {
        class BasicDitherRenderPass : ScriptableRenderPass
        {
            private Material material;
            private BasicDitherSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            public BasicDitherRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<BasicDitherSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/BasicDither"));
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

                if (settings.useSceneColor.value)
                {
                    cmd.EnableShaderKeyword("USE_SCENE_TEXTURE_ON");
                }
                else
                {
                    cmd.DisableShaderKeyword("USE_SCENE_TEXTURE_ON");
                }

                // Set BasicDither effect properties.
                cmd.SetGlobalTexture("_NoiseTex", settings.noiseTex.value ?? Texture2D.whiteTexture);
                cmd.SetGlobalFloat("_NoiseSize", settings.noiseSize.value);
                cmd.SetGlobalFloat("_ThresholdOffset", settings.thresholdOffset.value);
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

        BasicDitherRenderPass pass;

        public override void Create()
        {
            pass = new BasicDitherRenderPass("BasicDither");
            name = "Basic Dither";
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
