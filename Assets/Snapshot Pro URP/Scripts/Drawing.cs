namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Drawing : ScriptableRendererFeature
    {
        class DrawingRenderPass : ScriptableRenderPass
        {
            private Material material;
            private DrawingSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            public DrawingRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<DrawingSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Drawing"));
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

                bool isOffset = false;
                float animCycleTime = settings.animCycleTime.value;

                if (animCycleTime > 0.0f)
                {
                    isOffset = (Time.time % animCycleTime) < (animCycleTime / 2.0f);
                }

                // Set Drawing effect properties.
                cmd.SetGlobalTexture("_DrawingTex", settings.drawingTex.value ?? Texture2D.whiteTexture);
                cmd.SetGlobalFloat("_OverlayOffset", isOffset ? 0.5f : 0.0f);
                cmd.SetGlobalFloat("_Strength", settings.strength.value);
                cmd.SetGlobalFloat("_Tiling", settings.tiling.value);
                cmd.SetGlobalFloat("_Smudge", settings.smudge.value);
                cmd.SetGlobalFloat("_DepthThreshold", settings.depthThreshold.value);

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

        DrawingRenderPass pass;

        public override void Create()
        {
            pass = new DrawingRenderPass("Drawing");
            name = "Drawing";
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
