namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Pixelate : ScriptableRendererFeature
    {
        class PixelateRenderPass : ScriptableRenderPass
        {
            private PixelateSettings settings;

            // private int pixelID;
            // private RenderTargetIdentifier pixelRT;

            private RenderTargetIdentifier source;
            private RenderTargetHandle pixelTex;
            private string profilerTag;

            public PixelateRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<PixelateSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (settings == null)
                {
                    return;
                }

                base.Configure(cmd, cameraTextureDescriptor);

                int width = cameraTextureDescriptor.width / settings.pixelSize.value;
                int height = cameraTextureDescriptor.height / settings.pixelSize.value;
                RenderTextureFormat format = cameraTextureDescriptor.colorFormat;

                pixelTex = new RenderTargetHandle();
                pixelTex.id = Shader.PropertyToID("PixelTex");
                cmd.GetTemporaryRT(pixelTex.id, width, height, 0, FilterMode.Point, format);

                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!settings.IsActive())
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

                // Copy camera color texture to pixelTex, then back again.
                cmd.Blit(source, pixelTex.id);
                cmd.Blit(pixelTex.id, source);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(pixelTex.id);
            }
        }

        PixelateRenderPass pass;

        public override void Create()
        {
            pass = new PixelateRenderPass("Pixelate");
            name = "Pixelate";
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
