using System.Collections;
using System.Runtime;
using UnityEngine;
using DistantLands.Cozy.Data;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class BlocksBiome : CozyBiomeModule
    {

        #region Runtime Values
        [ColorUsage(true, true)] public Color skyZenithColor;
        [ColorUsage(true, true)] public Color skyHorizonColor;
        [ColorUsage(true, true)] public Color cloudColor;
        [ColorUsage(true, true)] public Color cloudHighlightColor;
        [ColorUsage(true, true)] public Color highAltitudeCloudColor;
        [ColorUsage(true, true)] public Color sunlightColor;
        [ColorUsage(true, true)] public Color starColor;
        [ColorUsage(true, true)] public Color ambientLightHorizonColor;
        [ColorUsage(true, true)] public Color ambientLightZenithColor;
        public float galaxyIntensity;
        [ColorUsage(true, true)] public Color fogColor1;
        [ColorUsage(true, true)] public Color fogColor2;
        [ColorUsage(true, true)] public Color fogColor3;
        [ColorUsage(true, true)] public Color fogColor4;
        [ColorUsage(true, true)] public Color fogColor5;
        [ColorUsage(true, true)] public Color fogFlareColor;
        public float gradientExponent = 0.364f;
        public float ambientLightMultiplier;
        public float atmosphereVariationMin;
        public float atmosphereVariationMax;
        public float atmosphereBias = 1;
        public float sunSize = 0.7f;
        [ColorUsage(true, true)] public Color sunColor;
        public float sunFalloff = 43.7f;
        [ColorUsage(true, true)] public Color sunFlareColor;
        public float moonFalloff = 24.4f;
        [ColorUsage(true, true)] public Color moonlightColor;
        [ColorUsage(true, true)] public Color moonFlareColor;
        [ColorUsage(true, true)] public Color galaxy1Color;
        [ColorUsage(true, true)] public Color galaxy2Color;
        [ColorUsage(true, true)] public Color galaxy3Color;
        [ColorUsage(true, true)] public Color lightScatteringColor;
        public float fogStart1 = 2;
        public float fogStart2 = 5;
        public float fogStart3 = 10;
        public float fogStart4 = 30;
        public float fogHeight = 0.85f;
        public float fogDensityMultiplier;
        public float fogLightFlareIntensity = 1;
        public float fogLightFlareFalloff = 21;
        public float fogLightFlareSquish = 1;
        [ColorUsage(true, true)] public Color cloudMoonColor;
        [ColorUsage(true, true)] public Color cloudTextureColor;
        public float cloudSunHighlightFalloff = 14.1f;
        public float cloudMoonHighlightFalloff = 22.9f;

        public float rainbowIntensity;
        public BlocksModule parentModule;

        #endregion

        #region UI

        public bool selection;
        public bool blockSettings;
        #endregion


        public BlockProfile blockProfile;

        public List<BlocksModule.Block> blocks = new List<BlocksModule.Block>();
        public List<float> keys = new List<float>();


        public ColorBlock currentBlock;

        void Awake()
        {

            if (!biome)
                biome = GetComponent<CozyBiome>();

            parentModule = biome.weatherSphere.GetModule<BlocksModule>();
            GetBlocks();

        }

        void Start()
        {
            biome.weatherSphere = CozyWeather.instance;

            if (!biome.trigger)
                biome.CheckTrigger();

        }

        void Update()
        {

            if (biome.weatherSphere.freezeUpdateInEditMode && !Application.isPlaying)
                return;

            if (blocks.Count > 0)
                SetColorsFromBlocks();
            else if (blockProfile)
                GetBlocks();

        }

        public override void AddBiome()
        {
            parentModule.biomes.Add(this);
        }

        public override void RemoveBiome()
        {
            parentModule.biomes.Remove(this);
        }

        public void GetBlocks()
        {

            if (blockProfile == null)
                return;

            List<BlocksModule.Block> i = new List<BlocksModule.Block>();

            List<ColorBlock> j = new List<ColorBlock>();

            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.dawn))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.dawnBlock, blockProfile.dawn.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.morning))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.morningBlock, blockProfile.morning.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.day))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.dayBlock, blockProfile.day.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.afternoon))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.afternoonBlock, blockProfile.afternoon.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.evening))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.eveningBlock, blockProfile.evening.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.twilight))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.twilightBlock, blockProfile.twilight.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.night))
                i.Add(new BlocksModule.Block(biome.weatherSphere.perennialProfile.nightBlock, blockProfile.night.ToArray()));

            blocks = i;

            foreach (BlocksModule.Block k in blocks)
                k.GetColorBlock(biome.weatherSphere);

        }

        public void ApplyPropertiesToWeatherSphere()
        {

            biome.weatherSphere.gradientExponent += gradientExponent * biome.weight;
            biome.weatherSphere.ambientLightHorizonColor += ambientLightHorizonColor * biome.weight;
            biome.weatherSphere.ambientLightZenithColor += skyZenithColor * biome.weight;
            biome.weatherSphere.ambientLightMultiplier += ambientLightMultiplier * biome.weight;
            biome.weatherSphere.atmosphereBias += atmosphereBias * biome.weight;
            biome.weatherSphere.atmosphereVariationMax += atmosphereVariationMax * biome.weight;
            biome.weatherSphere.atmosphereVariationMin += atmosphereVariationMin * biome.weight;
            biome.weatherSphere.cloudColor += cloudColor * biome.weight;
            biome.weatherSphere.cloudHighlightColor += cloudHighlightColor * biome.weight;
            biome.weatherSphere.cloudMoonColor += cloudMoonColor * biome.weight;
            biome.weatherSphere.cloudMoonHighlightFalloff += cloudMoonHighlightFalloff * biome.weight;
            biome.weatherSphere.cloudSunHighlightFalloff += cloudSunHighlightFalloff * biome.weight;
            biome.weatherSphere.cloudTextureColor += cloudTextureColor * biome.weight;
            biome.weatherSphere.fogColor1 += fogColor1 * biome.weight;
            biome.weatherSphere.fogColor2 += fogColor2 * biome.weight;
            biome.weatherSphere.fogColor3 += fogColor3 * biome.weight;
            biome.weatherSphere.fogColor4 += fogColor4 * biome.weight;
            biome.weatherSphere.fogColor5 += fogColor5 * biome.weight;
            biome.weatherSphere.fogStart1 += fogStart1 * biome.weight;
            biome.weatherSphere.fogStart2 += fogStart2 * biome.weight;
            biome.weatherSphere.fogStart3 += fogStart3 * biome.weight;
            biome.weatherSphere.fogStart4 += fogStart4 * biome.weight;
            biome.weatherSphere.fogFlareColor += fogFlareColor * biome.weight;
            biome.weatherSphere.fogHeight += fogHeight * biome.weight;
            biome.weatherSphere.fogDensityMultiplier += fogDensityMultiplier * biome.weight;
            biome.weatherSphere.fogLightFlareFalloff += fogLightFlareFalloff * biome.weight;
            biome.weatherSphere.fogLightFlareIntensity += fogLightFlareIntensity * biome.weight;
            biome.weatherSphere.fogLightFlareSquish += fogLightFlareSquish * biome.weight;
            biome.weatherSphere.galaxy1Color += galaxy1Color * biome.weight;
            biome.weatherSphere.galaxy2Color += galaxy2Color * biome.weight;
            biome.weatherSphere.galaxy3Color += galaxy3Color * biome.weight;
            biome.weatherSphere.galaxyIntensity += galaxyIntensity * biome.weight;
            biome.weatherSphere.highAltitudeCloudColor += highAltitudeCloudColor * biome.weight;
            biome.weatherSphere.lightScatteringColor += lightScatteringColor * biome.weight;
            biome.weatherSphere.moonlightColor += moonlightColor * biome.weight;
            biome.weatherSphere.moonFalloff += moonFalloff * biome.weight;
            biome.weatherSphere.moonFlareColor += moonFlareColor * biome.weight;
            biome.weatherSphere.skyHorizonColor += skyHorizonColor * biome.weight;
            biome.weatherSphere.skyZenithColor += skyZenithColor * biome.weight;
            biome.weatherSphere.starColor += starColor * biome.weight;
            biome.weatherSphere.sunColor += sunColor * biome.weight;
            biome.weatherSphere.sunFalloff += sunFalloff * biome.weight;
            biome.weatherSphere.sunFlareColor += sunFlareColor * biome.weight;
            biome.weatherSphere.sunlightColor += sunlightColor * biome.weight;
            biome.weatherSphere.sunSize += sunSize * biome.weight;

        }

        void SetColorsFromBlocks()
        {

            float time = biome.weatherSphere.GetModifiedDayPercentage();


            ColorBlock currentBlock;


            if (keys.Count > 0)
                keys.Clear();

            foreach (BlocksModule.Block j in blocks)
            {

                if (j.colorBlocks.Length == 0)
                    continue;

                keys.Add(j.startKey);
                keys.Add(j.endKey);
            }

            int k = 0;

            foreach (float i in keys)
            {

                if (time > i)
                {
                    k++;

                    if (k == keys.Count)
                    {
                        SingleBlock(blocks[blocks.Count - 1].selectedBlock);
                        blocks[0].GetColorBlock(biome.weatherSphere);
                    }
                    continue;
                }

                currentBlock = k > 1 ? blocks[k / 2 - 1].selectedBlock : blocks[blocks.Count - 1].selectedBlock;

                if (k % 2 == 1)
                {

                    //In between two key frames from the same block.
                    TwoBlock(currentBlock, blocks[Mathf.FloorToInt(k / 2)].selectedBlock, (time - keys[k - 1]) / (i - keys[k - 1]));
                    break;

                }
                else
                {
                    //In between two key frames from different blocks.
                    SingleBlock(currentBlock);
                    if (k == keys.Count - 2)
                    {
                        blocks[0].seed = new System.Random().Next();
                        blocks[0].GetColorBlock(biome.weatherSphere);
                    }
                    else
                        blocks[Mathf.FloorToInt(k / 2) + 1].seed = new System.Random().Next();
                    blocks[Mathf.FloorToInt(k / 2) + 1].GetColorBlock(biome.weatherSphere);
                    break;

                }
            }
        }

        void SingleBlock(ColorBlock colorBlock)
        {

            if (colorBlock == null)
                return;

            ColorBlock block = colorBlock;

            gradientExponent = block.gradientExponent;
            ambientLightHorizonColor = block.ambientLightHorizonColor;
            ambientLightZenithColor = block.ambientLightZenithColor;
            ambientLightMultiplier = block.ambientLightMultiplier;
            atmosphereBias = block.atmosphereBias;
            atmosphereVariationMax = block.atmosphereVariationMax;
            atmosphereVariationMin = block.atmosphereVariationMin;
            cloudColor = block.cloudColor;
            cloudHighlightColor = block.cloudHighlightColor;
            cloudMoonColor = block.cloudMoonColor;
            cloudMoonHighlightFalloff = block.cloudMoonHighlightFalloff;
            cloudSunHighlightFalloff = block.cloudSunHighlightFalloff;
            cloudTextureColor = block.cloudTextureColor;
            fogColor1 = block.fogColor1;
            fogColor2 = block.fogColor2;
            fogColor3 = block.fogColor3;
            fogColor4 = block.fogColor4;
            fogColor5 = block.fogColor5;
            fogStart1 = block.fogStart1;
            fogStart2 = block.fogStart2;
            fogStart3 = block.fogStart3;
            fogStart4 = block.fogStart4;
            fogFlareColor = block.fogFlareColor;
            fogHeight = block.fogHeight;
            fogDensityMultiplier = block.fogDensity;
            fogLightFlareFalloff = block.fogLightFlareFalloff;
            fogLightFlareIntensity = block.fogLightFlareIntensity;
            fogLightFlareSquish = block.fogLightFlareSquish;
            galaxy1Color = block.galaxy1Color;
            galaxy2Color = block.galaxy2Color;
            galaxy3Color = block.galaxy3Color;
            galaxyIntensity = block.galaxyIntensity;
            highAltitudeCloudColor = block.highAltitudeCloudColor;
            lightScatteringColor = block.lightScatteringColor;
            moonlightColor = block.moonlightColor;
            moonFalloff = block.moonFalloff;
            moonFlareColor = block.moonFlareColor;
            skyHorizonColor = block.skyHorizonColor;
            skyZenithColor = block.skyZenithColor;
            starColor = block.starColor;
            sunColor = block.sunColor;
            sunFalloff = block.sunFalloff;
            sunFlareColor = block.sunFlareColor;
            sunlightColor = block.sunlightColor;
            sunSize = block.sunSize;

            currentBlock = colorBlock;

        }

        void TwoBlock(ColorBlock colorBlock1, ColorBlock colorBlock2, float value)
        {
            if (colorBlock1 == null || colorBlock2 == null)
                return;

            ColorBlock block1 = colorBlock1;
            ColorBlock block2 = colorBlock2;

            gradientExponent = Mathf.Lerp(block1.gradientExponent, block2.gradientExponent, value);
            ambientLightHorizonColor = Color.Lerp(block1.ambientLightHorizonColor, block2.ambientLightHorizonColor, value);
            ambientLightZenithColor = Color.Lerp(block1.ambientLightZenithColor, block2.ambientLightZenithColor, value);
            ambientLightMultiplier = Mathf.Lerp(block1.ambientLightMultiplier, block2.ambientLightMultiplier, value);
            atmosphereBias = Mathf.Lerp(block1.atmosphereBias, block2.atmosphereBias, value);
            atmosphereVariationMax = Mathf.Lerp(block1.atmosphereVariationMax, block2.atmosphereVariationMax, value);
            atmosphereVariationMin = Mathf.Lerp(block1.atmosphereVariationMin, block2.atmosphereVariationMin, value);
            cloudColor = Color.Lerp(block1.cloudColor, block2.cloudColor, value);
            cloudHighlightColor = Color.Lerp(block1.cloudHighlightColor, block2.cloudHighlightColor, value);
            cloudMoonColor = Color.Lerp(block1.cloudMoonColor, block2.cloudMoonColor, value);
            cloudMoonHighlightFalloff = Mathf.Lerp(block1.cloudMoonHighlightFalloff, block2.cloudMoonHighlightFalloff, value);
            cloudSunHighlightFalloff = Mathf.Lerp(block1.cloudSunHighlightFalloff, block2.cloudSunHighlightFalloff, value);
            cloudTextureColor = Color.Lerp(block1.cloudTextureColor, block2.cloudTextureColor, value);
            fogColor1 = Color.Lerp(block1.fogColor1, block2.fogColor1, value);
            fogColor2 = Color.Lerp(block1.fogColor2, block2.fogColor2, value);
            fogColor3 = Color.Lerp(block1.fogColor3, block2.fogColor3, value);
            fogColor4 = Color.Lerp(block1.fogColor4, block2.fogColor4, value);
            fogColor5 = Color.Lerp(block1.fogColor5, block2.fogColor5, value);
            fogStart1 = Mathf.Lerp(block1.fogStart1, block2.fogStart1, value);
            fogStart2 = Mathf.Lerp(block1.fogStart2, block2.fogStart2, value);
            fogStart3 = Mathf.Lerp(block1.fogStart3, block2.fogStart3, value);
            fogStart4 = Mathf.Lerp(block1.fogStart4, block2.fogStart4, value);
            fogFlareColor = Color.Lerp(block1.fogFlareColor, block2.fogFlareColor, value);
            fogHeight = Mathf.Lerp(block1.fogHeight, block2.fogHeight, value);
            fogDensityMultiplier = Mathf.Lerp(block1.fogDensity, block2.fogDensity, value);
            fogLightFlareFalloff = Mathf.Lerp(block1.fogLightFlareFalloff, block2.fogLightFlareFalloff, value);
            fogLightFlareIntensity = Mathf.Lerp(block1.fogLightFlareIntensity, block2.fogLightFlareIntensity, value);
            fogLightFlareSquish = Mathf.Lerp(block1.fogLightFlareSquish, block2.fogLightFlareSquish, value);
            galaxy1Color = Color.Lerp(block1.galaxy1Color, block2.galaxy1Color, value);
            galaxy2Color = Color.Lerp(block1.galaxy2Color, block2.galaxy2Color, value);
            galaxy3Color = Color.Lerp(block1.galaxy3Color, block2.galaxy3Color, value);
            galaxyIntensity = Mathf.Lerp(block1.galaxyIntensity, block2.galaxyIntensity, value);
            highAltitudeCloudColor = Color.Lerp(block1.highAltitudeCloudColor, block2.highAltitudeCloudColor, value);
            lightScatteringColor = Color.Lerp(block1.lightScatteringColor, block2.lightScatteringColor, value);
            moonlightColor = Color.Lerp(block1.moonlightColor, block2.moonlightColor, value);
            moonFalloff = Mathf.Lerp(block1.moonFalloff, block2.moonFalloff, value);
            moonFlareColor = Color.Lerp(block1.moonFlareColor, block2.moonFlareColor, value);
            skyHorizonColor = Color.Lerp(block1.skyHorizonColor, block2.skyHorizonColor, value);
            skyZenithColor = Color.Lerp(block1.skyZenithColor, block2.skyZenithColor, value);
            starColor = Color.Lerp(block1.starColor, block2.starColor, value);
            sunColor = Color.Lerp(block1.sunColor, block2.sunColor, value);
            sunFalloff = Mathf.Lerp(block1.sunFalloff, block2.sunFalloff, value);
            sunFlareColor = Color.Lerp(block1.sunFlareColor, block2.sunFlareColor, value);
            sunlightColor = Color.Lerp(block1.sunlightColor, block2.sunlightColor, value);
            sunSize = Mathf.Lerp(block1.sunSize, block2.sunSize, value);

            currentBlock = value > 0.5 ? colorBlock2 : colorBlock1;

        }

        public override bool CheckBiome()
        {

            if (!CozyWeather.instance.GetModule<BlocksModule>())
            {
                Debug.LogError("The BLOCKS biome module requires the BLOCKS module to be enabled on your weather sphere. Please add the BLOCKS module before setting up your biome.");
                return false;
            }
            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BlocksBiome))]
    [CanEditMultipleObjects]
    public class E_BlocksBiome : E_BiomeModule
    {

        BlocksBiome t;
        protected static bool blocksFoldout;

        void OnEnable()
        {

            t = (BlocksBiome)target;

        }

        public override void DrawInlineUI(GUIStyle foldoutStyle)
        {
            serializedObject.Update();

            blocksFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(blocksFoldout, "   BLOCKS", foldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (blocksFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("blockProfile"));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.Space();
                if (t.blockProfile)
                    (Editor.CreateEditor(t.blockProfile) as E_BlockProfile).EditInline(t);
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();

        }

        public override void DrawReports()
        {
            EditorGUILayout.HelpBox($"Current BLOCK is {t.currentBlock.name}", MessageType.None);
        }
    }

#endif
}