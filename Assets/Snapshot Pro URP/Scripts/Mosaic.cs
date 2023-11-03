namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Mosaic : ScriptableRendererFeature
    {
        class MosaicRenderPass : ScriptableRenderPass
        {
            private Material material;
            private MosaicSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private RenderTargetHandle mosaicTex;
            private string profilerTag;

            private int xTileCount;
            private int yTileCount;

            public MosaicRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<MosaicSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Mosaic"));
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

                int screenWidth = cameraTextureDescriptor.width;
                int screenHeight = cameraTextureDescriptor.height;
                RenderTextureFormat format = cameraTextureDescriptor.colorFormat;

                xTileCount = settings.xTileCount.value;
                yTileCount = Mathf.RoundToInt((float)screenHeight / screenWidth * xTileCount);
                FilterMode filterMode = settings.usePointFiltering.value ? FilterMode.Point : FilterMode.Bilinear;

                mosaicTex = new RenderTargetHandle();
                mosaicTex.id = Shader.PropertyToID("MosaicTex");
                cmd.GetTemporaryRT(mosaicTex.id, xTileCount, yTileCount, 0, filterMode, format);

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

                // Set Mosaic effect properties.
                cmd.SetGlobalTexture("_OverlayTex", settings.overlayTexture.value ?? Texture2D.whiteTexture);
                cmd.SetGlobalColor("_OverlayColor", settings.overlayColor.value);
                cmd.SetGlobalInt("_XTileCount", xTileCount);
                cmd.SetGlobalInt("_YTileCount", yTileCount);

                // Execute effect using effect material.
                cmd.Blit(mainTex.id, mosaicTex.id);
                cmd.Blit(mosaicTex.id, source, material);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(mainTex.id);
                cmd.ReleaseTemporaryRT(mosaicTex.id);
            }
        }

        MosaicRenderPass pass;

        public override void Create()
        {
            pass = new MosaicRenderPass("Mosaic");
            name = "Mosaic";
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
