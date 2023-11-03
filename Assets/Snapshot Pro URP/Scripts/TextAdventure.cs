namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    public class TextAdventure : ScriptableRendererFeature
    {
        class TextAdventureRenderPass : ScriptableRenderPass
        {
            private Material material;
            private TextAdventureSettings settings;

            private RenderTargetIdentifier source;
            private RenderTargetHandle mainTex;
            private string profilerTag;

            private Vector2Int pixelSize;

            public TextAdventureRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                source = renderer.cameraColorTargetHandle;
                settings = VolumeManager.instance.stack.GetComponent<TextAdventureSettings>();
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }

            public void EnqueuePass(ScriptableRenderer renderer)
            {
                if (settings != null && settings.IsActive())
                {
                    renderer.EnqueuePass(this);
                    material = new Material(Shader.Find("SnapshotProURP/TextAdventure"));
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (settings == null)
                {
                    return;
                }

                float size = settings.characterSize.value;
                float aspect = (float)Screen.height / Screen.width;
                pixelSize = new Vector2Int(Mathf.CeilToInt((Screen.width) / size),
                    Mathf.CeilToInt(Screen.height / size));

                var descriptor = cameraTextureDescriptor;
                descriptor.width = pixelSize.x;
                descriptor.height = pixelSize.y;

                mainTex = new RenderTargetHandle();
                mainTex.id = Shader.PropertyToID("MainTex");
                cmd.GetTemporaryRT(mainTex.id, descriptor);

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

                // Set Text Adventure effect properties.
                cmd.SetGlobalTexture("_CharacterAtlas", settings.characterAtlas.value);
                cmd.SetGlobalInteger("_CharacterCount", settings.characterCount.value);
                cmd.SetGlobalVector("_CharacterSize", (Vector2)pixelSize);
                cmd.SetGlobalColor("_BackgroundColor", settings.backgroundColor.value);
                cmd.SetGlobalColor("_CharacterColor", settings.characterColor.value);

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

        TextAdventureRenderPass pass;

        public override void Create()
        {
            pass = new TextAdventureRenderPass("Text Adventure");
            name = "Text Adventure";
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
