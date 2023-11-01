namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Blur : ScriptableRendererFeature
    {
        class BlurRenderPass : ScriptableRenderPass
        {
            private Material material;
            private BlurSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private RenderTargetHandle blurTex;
            private string profilerTag;

            public BlurRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Blur"));
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

                blurTex = new RenderTargetHandle();
                blurTex.id = Shader.PropertyToID("_BlurTex");
                cmd.GetTemporaryRT(blurTex.id, cameraTextureDescriptor);

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

                // Set Blur effect properties.
                cmd.SetGlobalInt("_KernelSize", settings.strength.value);
                cmd.SetGlobalFloat("_Spread", settings.strength.value / 7.5f);

                // Execute effect using effect material with two passes.
                cmd.Blit(mainTex.id, blurTex.id, material, 0);
                cmd.Blit(blurTex.id, source, material, 1);

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

        BlurRenderPass pass;

        public override void Create()
        {
            pass = new BlurRenderPass("Blur");
            name = "Blur";
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
