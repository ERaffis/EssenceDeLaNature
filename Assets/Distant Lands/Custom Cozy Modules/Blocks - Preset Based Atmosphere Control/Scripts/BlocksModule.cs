using System.Linq;
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
    public class BlocksModule : CozyModule
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

        #endregion

        #region UI

        public bool selection;
        public bool developmentToolkit;
        public bool blockSettings;
        #endregion

        public BlockProfile blockProfile;

        public List<Block> blocks = new List<Block>();
        public List<float> keys = new List<float>();
        [System.Serializable]
        public class Block
        {

            [Range(0, 1)]
            public float startKey;

            [Range(0, 1)]
            public float endKey;

            public ColorBlock[] colorBlocks;

            [HideInInspector]
            public int seed;


            public ColorBlock selectedBlock;
            public void GetColorBlock(CozyWeather weather)
            {

                if (colorBlocks.Length <= 0)
                    return;

                ColorBlock i = null;
                List<float> floats = new List<float>();
                float totalChance = 0;

                foreach (ColorBlock k in colorBlocks)
                {
                    floats.Add(k.GetChance(weather));
                    totalChance += k.GetChance(weather);
                }

                if (totalChance == 0)
                {
                    selectedBlock = colorBlocks[0];
                    return;
                }


                float selection = (float)new System.Random(seed).NextDouble() * totalChance;



                int m = 0;
                float l = 0;

                while (l <= selection)
                {

                    if (selection >= l && selection < l + floats[m])
                    {
                        i = colorBlocks[m];
                        break;
                    }
                    l += floats[m];
                    m++;

                }

                if (!i)
                {
                    i = colorBlocks[0];
                }

                selectedBlock = i;
            }

            public Block(float _startKey, float _endKey, ColorBlock[] _blocks)
            {

                startKey = _startKey;
                endKey = _endKey;
                colorBlocks = _blocks;

            }
            public Block(PerennialProfile.TimeBlock block, ColorBlock[] _blocks)
            {

                startKey = block.start;
                endKey = block.end;
                colorBlocks = _blocks;

            }

        }

        public ColorBlock currentBlock;
        public ColorBlock testColorBlock;

        public List<BlocksBiome> biomes = new List<BlocksBiome>();
        public float weight;

        void OnEnable()
        {

            base.SetupModule();
            weatherSphere.overrideAtmosphere = this;
            weatherSphere.atmosphereControl = CozyWeather.AtmosphereSelection.native;

        }

        void Awake()
        {

            testColorBlock = null;
            GetBlocks();

            biomes = FindObjectsOfType<BlocksBiome>().ToList();

        }

        void ManageBiomeWeights()
        {

            float j = 0;
            biomes.RemoveAll(x => x == null);

            foreach (BlocksBiome i in biomes)
            {

                j += i.biome.weight;

            }

            weight = Mathf.Clamp01(1 - j);

        }

        void Update()
        {

            if (testColorBlock)
                SingleBlock(testColorBlock);
            else if (blocks.Count > 0)
                SetColorsFromBlocks();
            else if (blockProfile)
                GetBlocks();

            ManageBiomeWeights();
            ApplyPropertiesToWeatherSphere();

        }

        public void ApplyPropertiesToWeatherSphere()
        {

            ResetWeatherSphere();

            weatherSphere.gradientExponent += gradientExponent * weight;
            weatherSphere.ambientLightHorizonColor += ambientLightHorizonColor * weight;
            weatherSphere.ambientLightZenithColor += skyZenithColor * weight;
            weatherSphere.ambientLightMultiplier += ambientLightMultiplier * weight;
            weatherSphere.atmosphereBias += atmosphereBias * weight;
            weatherSphere.atmosphereVariationMax += atmosphereVariationMax * weight;
            weatherSphere.atmosphereVariationMin += atmosphereVariationMin * weight;
            weatherSphere.cloudColor += cloudColor * weight;
            weatherSphere.cloudHighlightColor += cloudHighlightColor * weight;
            weatherSphere.cloudMoonColor += cloudMoonColor * weight;
            weatherSphere.cloudMoonHighlightFalloff += cloudMoonHighlightFalloff * weight;
            weatherSphere.cloudSunHighlightFalloff += cloudSunHighlightFalloff * weight;
            weatherSphere.cloudTextureColor += cloudTextureColor * weight;
            weatherSphere.fogColor1 += fogColor1 * weight;
            weatherSphere.fogColor2 += fogColor2 * weight;
            weatherSphere.fogColor3 += fogColor3 * weight;
            weatherSphere.fogColor4 += fogColor4 * weight;
            weatherSphere.fogColor5 += fogColor5 * weight;
            weatherSphere.fogStart1 += fogStart1 * weight;
            weatherSphere.fogStart2 += fogStart2 * weight;
            weatherSphere.fogStart3 += fogStart3 * weight;
            weatherSphere.fogStart4 += fogStart4 * weight;
            weatherSphere.fogFlareColor += fogFlareColor * weight;
            weatherSphere.fogHeight += fogHeight * weight;
            weatherSphere.fogDensityMultiplier += fogDensityMultiplier * weight;
            weatherSphere.fogLightFlareFalloff += fogLightFlareFalloff * weight;
            weatherSphere.fogLightFlareIntensity += fogLightFlareIntensity * weight;
            weatherSphere.fogLightFlareSquish += fogLightFlareSquish * weight;
            weatherSphere.galaxy1Color += galaxy1Color * weight;
            weatherSphere.galaxy2Color += galaxy2Color * weight;
            weatherSphere.galaxy3Color += galaxy3Color * weight;
            weatherSphere.galaxyIntensity += galaxyIntensity * weight;
            weatherSphere.highAltitudeCloudColor += highAltitudeCloudColor * weight;
            weatherSphere.lightScatteringColor += lightScatteringColor * weight;
            weatherSphere.moonlightColor += moonlightColor * weight;
            weatherSphere.moonFalloff += moonFalloff * weight;
            weatherSphere.moonFlareColor += moonFlareColor * weight;
            weatherSphere.skyHorizonColor += skyHorizonColor * weight;
            weatherSphere.skyZenithColor += skyZenithColor * weight;
            weatherSphere.starColor += starColor * weight;
            weatherSphere.sunColor += sunColor * weight;
            weatherSphere.sunFalloff += sunFalloff * weight;
            weatherSphere.sunFlareColor += sunFlareColor * weight;
            weatherSphere.sunlightColor += sunlightColor * weight;
            weatherSphere.sunSize += sunSize * weight;

            BlocksBiome biome;

            for (int j = 0; j < biomes.Count; j++)
            {
                biome = biomes[j];

                if (biome.biome.weight > 0)
                {
                    biome.ApplyPropertiesToWeatherSphere();
                }
            }

        }

        void ResetWeatherSphere()
        {
            
            weatherSphere.gradientExponent = 0;
            weatherSphere.ambientLightHorizonColor = Color.clear;
            weatherSphere.ambientLightZenithColor = Color.clear;
            weatherSphere.ambientLightMultiplier = 0;
            weatherSphere.atmosphereBias = 0;
            weatherSphere.atmosphereVariationMax = 0;
            weatherSphere.atmosphereVariationMin = 0;
            weatherSphere.cloudColor = Color.clear;
            weatherSphere.cloudHighlightColor = Color.clear;
            weatherSphere.cloudMoonColor = Color.clear;
            weatherSphere.cloudMoonHighlightFalloff = 0;
            weatherSphere.cloudSunHighlightFalloff = 0;
            weatherSphere.cloudTextureColor = Color.clear;
            weatherSphere.fogColor1 = Color.clear;
            weatherSphere.fogColor2 = Color.clear;
            weatherSphere.fogColor3 = Color.clear;
            weatherSphere.fogColor4 = Color.clear;
            weatherSphere.fogColor5 = Color.clear;
            weatherSphere.fogStart1 = 0;
            weatherSphere.fogStart2 = 0;
            weatherSphere.fogStart3 = 0;
            weatherSphere.fogStart4 = 0;
            weatherSphere.fogFlareColor = Color.clear;
            weatherSphere.fogHeight = 0;
            weatherSphere.fogDensityMultiplier = 0;
            weatherSphere.fogLightFlareFalloff = 0;
            weatherSphere.fogLightFlareIntensity = 0;
            weatherSphere.fogLightFlareSquish = 0;
            weatherSphere.galaxy1Color = Color.clear;
            weatherSphere.galaxy2Color = Color.clear;
            weatherSphere.galaxy3Color = Color.clear;
            weatherSphere.galaxyIntensity = 0;
            weatherSphere.highAltitudeCloudColor = Color.clear;
            weatherSphere.lightScatteringColor = Color.clear;
            weatherSphere.moonlightColor = Color.clear;
            weatherSphere.moonFalloff = 0;
            weatherSphere.moonFlareColor = Color.clear;
            weatherSphere.skyHorizonColor = Color.clear;
            weatherSphere.skyZenithColor = Color.clear;
            weatherSphere.starColor = Color.clear;
            weatherSphere.sunColor = Color.clear;
            weatherSphere.sunFalloff = 0;
            weatherSphere.sunFlareColor = Color.clear;
            weatherSphere.sunlightColor = Color.clear;
            weatherSphere.sunSize = 0;
        }

        public void GetBlocks()
        {

            if (blockProfile == null)
                return;

            List<Block> i = new List<Block>();

            List<ColorBlock> j = new List<ColorBlock>();

            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.dawn))
                i.Add(new Block(weatherSphere.perennialProfile.dawnBlock, blockProfile.dawn.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.morning))
                i.Add(new Block(weatherSphere.perennialProfile.morningBlock, blockProfile.morning.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.day))
                i.Add(new Block(weatherSphere.perennialProfile.dayBlock, blockProfile.day.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.afternoon))
                i.Add(new Block(weatherSphere.perennialProfile.afternoonBlock, blockProfile.afternoon.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.evening))
                i.Add(new Block(weatherSphere.perennialProfile.eveningBlock, blockProfile.evening.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.twilight))
                i.Add(new Block(weatherSphere.perennialProfile.twilightBlock, blockProfile.twilight.ToArray()));
            if (blockProfile.timeBlocks.HasFlag(BlockProfile.TimeBlocks.night))
                i.Add(new Block(weatherSphere.perennialProfile.nightBlock, blockProfile.night.ToArray()));

            blocks = i;

            foreach (Block k in blocks)
                k.GetColorBlock(weatherSphere);

        }

        void SetColorsFromBlocks()
        {

            float time = weatherSphere.GetModifiedDayPercentage();


            ColorBlock currentBlock;


            if (keys.Count > 0)
                keys.Clear();

            foreach (Block j in blocks)
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
                        blocks[0].GetColorBlock(weatherSphere);
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
                        blocks[0].GetColorBlock(weatherSphere);
                    }
                    else
                        blocks[Mathf.FloorToInt(k / 2) + 1].seed = new System.Random().Next();
                    blocks[Mathf.FloorToInt(k / 2) + 1].GetColorBlock(weatherSphere);
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

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BlocksModule))]
    [CanEditMultipleObjects]
    public class E_BlocksModule : E_CozyModule
    {

        BlocksModule t;

        public override GUIContent GetGUIContent()
        {
            return new GUIContent("    Blocks", (Texture)Resources.Load("Blocks"), "Override the atmosphere colors with a preset based GUI.");
        }

        void OnEnable()
        {

            t = (BlocksModule)target;

        }

        public override void DisplayInCozyWindow()
        {
            serializedObject.Update();

            serializedObject.FindProperty("selection").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("selection").boolValue, "    Selection Settings", EditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (serializedObject.FindProperty("selection").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("blockProfile"));
                serializedObject.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }

            if (serializedObject.FindProperty("blockProfile").objectReferenceValue)
            {
                serializedObject.FindProperty("blockSettings").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("blockSettings").boolValue, "    Color Blocks", EditorUtilities.FoldoutStyle());
                EditorGUILayout.EndFoldoutHeaderGroup();

                if (serializedObject.FindProperty("blockSettings").boolValue)
                {

                    EditorGUI.indentLevel++;
                    (Editor.CreateEditor(t.blockProfile) as E_BlockProfile).EditInline(t);
                    EditorGUI.indentLevel--;

                }

                serializedObject.FindProperty("developmentToolkit").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedObject.FindProperty("developmentToolkit").boolValue, "    Development Toolkit", EditorUtilities.FoldoutStyle());
                EditorGUILayout.EndFoldoutHeaderGroup();

                if (serializedObject.FindProperty("developmentToolkit").boolValue)
                {

                    EditorGUI.indentLevel++;
                    if (t.currentBlock)
                        EditorGUILayout.LabelField($"Current Block is {t.currentBlock.name}");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("testColorBlock"));
                    EditorGUILayout.Space();
                    t.weatherSphere.currentTicks = EditorGUILayout.Slider("Current Time", t.weatherSphere.currentTicks, 0, t.weatherSphere.perennialProfile.ticksPerDay);
                    t.weatherSphere.currentDay = EditorGUILayout.IntSlider("Current Day", t.weatherSphere.currentDay, 0, t.weatherSphere.perennialProfile.daysPerYear);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Reset Block Times"))
                        t.GetBlocks();
                    EditorGUI.indentLevel--;

                }

                if (t.testColorBlock)
                    Editor.CreateEditor(t.testColorBlock).OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();

        }

    }

#endif
}