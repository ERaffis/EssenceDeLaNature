namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Vortex : ScriptableRendererFeature
    {
        class VortexRenderPass : ScriptableRenderPass
        {
            private Material material;
            private VortexSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            public VortexRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<VortexSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Vortex"));
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

                // Set Vortex effect properties.
                cmd.SetGlobalVector("_Center", settings.center.value);
                cmd.SetGlobalFloat("_Strength", settings.strength.value);
                cmd.SetGlobalVector("_Offset", settings.offset.value);

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

        VortexRenderPass pass;

        public override void Create()
        {
            pass = new VortexRenderPass("Vortex");
            name = "Vortex";
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
