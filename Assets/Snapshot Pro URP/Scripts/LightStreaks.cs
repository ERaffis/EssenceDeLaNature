namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class LightStreaks : ScriptableRendererFeature
    {
        class LightStreaksRenderPass : ScriptableRenderPass
        {
            private Material material;
            private LightStreaksSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private RenderTargetHandle blurTex;
            private string profilerTag;

            public LightStreaksRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<LightStreaksSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/LightStreaks"));
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (settings == null)
                {
                    return;
                }

                mainTex = new RenderTargetHandle();
                mainTex.id = Shader.PropertyToID("_MainTex");
                cmd.GetTemporaryRT(mainTex.id, cameraTextureDescriptor);

                var blurTextureDescriptor = cameraTextureDescriptor;
                blurTextureDescriptor.height /= 4;
                blurTextureDescriptor.width /= 4;

                blurTex = new RenderTargetHandle();
                blurTex.id = Shader.PropertyToID("_BlurTex");
                cmd.GetTemporaryRT(blurTex.id, blurTextureDescriptor);

                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!settings.IsActive())
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

                // Copy camera color texture to mainTex.
                cmd.Blit(source, mainTex.id);

                cmd.SetGlobalInt("_KernelSize", settings.strength.value);
                cmd.SetGlobalFloat("_Spread", settings.strength.value / 7.5f);
                cmd.SetGlobalFloat("_LuminanceThreshold", settings.luminanceThreshold.value);

                // Execute effect using effect material with two passes.
                cmd.Blit(mainTex.id, blurTex.id, material, 0);
                cmd.SetGlobalTexture("_BlurTex", blurTex.id);
                cmd.Blit(mainTex.id, source, material, 1);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(mainTex.id);
                cmd.ReleaseTemporaryRT(blurTex.id);
            }
        }

        LightStreaksRenderPass pass;

        public override void Create()
        {
            pass = new LightStreaksRenderPass("Light Streaks");
            name = "Light Streaks";
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
