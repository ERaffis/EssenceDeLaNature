namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class RadialBlur : ScriptableRendererFeature
    {
        class RadialBlurRenderPass : ScriptableRenderPass
        {
            private Material material;
            private RadialBlurSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private RenderTargetHandle blurTex;
            private string profilerTag;

            public RadialBlurRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<RadialBlurSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/RadialBlur"));
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
                CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

                // Copy camera color texture to mainTex.
                cmd.Blit(source, mainTex.id);

                // Set Radial Blur effect properties.
                cmd.SetGlobalInt("_KernelSize", settings.strength.value);
                cmd.SetGlobalFloat("_Spread", settings.strength.value / 7.5f);
                cmd.SetGlobalFloat("_FocalSize", settings.focalSize.value);

                // Execute effect using effect material with two passes.
                cmd.Blit(mainTex.id, blurTex.id, material, 0);
                cmd.Blit(blurTex.id, source, material, 1);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }

        RadialBlurRenderPass pass;

        public override void Create()
        {
            pass = new RadialBlurRenderPass("Radial Blur");
            name = "Radial Blur";
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
