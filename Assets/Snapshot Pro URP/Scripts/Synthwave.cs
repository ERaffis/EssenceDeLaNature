namespace SnapshotShaders.URP
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class Synthwave : ScriptableRendererFeature
    {
        class SynthwaveRenderPass : ScriptableRenderPass
        {
            private Material material;
            private SynthwaveSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            private static Dictionary<AxisMask, Vector3> axisMasks = new Dictionary<AxisMask, Vector3>
            {
                { AxisMask.XY, new Vector3(1.0f, 1.0f, 0.0f) },
                { AxisMask.XZ, new Vector3(1.0f, 0.0f, 1.0f) },
                { AxisMask.YZ, new Vector3(0.0f, 1.0f, 1.0f) },
                { AxisMask.XYZ, new Vector3(1.0f, 1.0f, 1.0f) }
            };

            public SynthwaveRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<SynthwaveSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/Synthwave"));
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

                // Set Synthwave effect properties.
                if (settings.useSceneColor.value == true)
                {
                    material.EnableKeyword("USE_SCENE_TEXTURE_ON");
                }
                else
                {
                    material.DisableKeyword("USE_SCENE_TEXTURE_ON");
                    cmd.SetGlobalColor("_BackgroundColor", settings.backgroundColor.value);
                }
                
                cmd.SetGlobalColor("_LineColor1", settings.lineColor1.value);
                cmd.SetGlobalColor("_LineColor2", settings.lineColor2.value);
                cmd.SetGlobalFloat("_LineColorMix", settings.lineColorMix.value);
                cmd.SetGlobalFloat("_LineWidth", settings.lineWidth.value);
                cmd.SetGlobalFloat("_LineFalloff", settings.lineFalloff.value);
                cmd.SetGlobalVector("_GapWidth", settings.gapWidth.value);
                cmd.SetGlobalVector("_Offset", settings.offset.value);
                cmd.SetGlobalVector("_AxisMask", axisMasks[settings.axisMask.value]);

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

        SynthwaveRenderPass pass;

        public override void Create()
        {
            pass = new SynthwaveRenderPass("Synthwave");
            name = "Synthwave";
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
