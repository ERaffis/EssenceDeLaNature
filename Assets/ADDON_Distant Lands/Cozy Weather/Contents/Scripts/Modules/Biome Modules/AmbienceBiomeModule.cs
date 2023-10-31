using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DistantLands.Cozy.Data;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    public class AmbienceBiomeModule : CozyBiomeModule
    {
        public AmbienceProfile currentAmbienceProfile;
        public CozyAmbienceManager parentModule;

        void Awake()
        {

            if (!biome)
                biome = GetComponent<CozyBiome>();

            if (parentModule == null)
                parentModule = biome.weatherSphere.GetModule<CozyAmbienceManager>();

        }
        public override void AddBiome()
        {
            if (parentModule == null)
                parentModule = biome.weatherSphere.GetModule<CozyAmbienceManager>();

            parentModule.biomes.Add(this);
        }

        public override bool CheckBiome()
        {

            if (!CozyWeather.instance.GetModule<CozyAmbienceManager>())
            {
                Debug.LogError("The ambience biome module requires the ambience module to be enabled on your weather sphere. Please add the ambience module before setting up your biome.");
                return false;
            }
            return true;
        }

        public override void RemoveBiome()
        {
            parentModule.biomes.Remove(this);
        }

        void Update()
        {
            if (Application.isPlaying)
            {

                currentAmbienceProfile.SetWeight(biome.weight);

            }
        }

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(AmbienceBiomeModule))]
    [CanEditMultipleObjects]
    public class E_AmbienceBiome : E_BiomeModule
    {

        AmbienceBiomeModule ambienceManager;
        protected static bool profileSettings;

        void OnEnable()
        {
            ambienceManager = (AmbienceBiomeModule)target;
        }

        public override void DrawInlineUI(GUIStyle foldoutStyle)
        {
            serializedObject.Update();

            profileSettings = EditorGUILayout.BeginFoldoutHeaderGroup(profileSettings, "   Ambience", EditorUtilities.FoldoutStyle());
            EditorGUI.EndFoldoutHeaderGroup();
            if (profileSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAmbienceProfile"));
                EditorGUI.indentLevel--;

            }

            serializedObject.ApplyModifiedProperties();

        }

        public override void DrawReports()
        {
            EditorGUILayout.HelpBox($"Current ambience is {ambienceManager.currentAmbienceProfile.name}", MessageType.None, true);
        }
    }

#endif
}