namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Cutout : ScriptableRendererFeature
    {
        class CutoutRenderPass : ScriptableRenderPass
        {
            private Material material;
            private CutoutSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            public CutoutRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<CutoutSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Cutout"));
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

                Matrix4x4 rotationMatrix = Matrix4x4.identity;
                rotationMatrix[0, 0] = rotationMatrix[1, 1] = Mathf.Cos(settings.rotation.value * Mathf.Deg2Rad);
                rotationMatrix[0, 1] = Mathf.Sin(settings.rotation.value * Mathf.Deg2Rad);
                rotationMatrix[1, 0] = -rotationMatrix[0, 1];

                // Set Cutout effect properties.
                cmd.SetGlobalTexture("_CutoutTex", settings.cutoutTexture.value);
                cmd.SetGlobalColor("_BorderColor", settings.borderColor.value);
                cmd.SetGlobalInt("_Stretch", settings.stretch.value ? 1 : 0);
                cmd.SetGlobalFloat("_Zoom", settings.zoom.value);
                cmd.SetGlobalVector("_Offset", settings.offset.value);
                cmd.SetGlobalMatrix("_Rotation", rotationMatrix);
                cmd.Blit(source, source, material);

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

        CutoutRenderPass pass;

        public override void Create()
        {
            pass = new CutoutRenderPass("Cutout");
            name = "Cutout";
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
