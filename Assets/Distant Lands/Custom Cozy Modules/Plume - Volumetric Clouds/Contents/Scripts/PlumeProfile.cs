using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Plume Profile", order = 361)]
    public class PlumeProfile : ScriptableObject
    {


        [Header("Particle Settings")]
        [Range(1, 30)]
        [Tooltip("Controls how many particles the individual cloud chunks will spawn")]
        public float cloudDensity = 10;
        [Tooltip("Controls the size of the cloud particles")]
        [Range(10, 200)]
        public float cloudParticleSize = 100;

        [Header("Generation Settings")]
        [Range(50, 500)]
        [Tooltip("Controls the size of the individual cloud chunks")]
        public float chunkSize = 150;
        [Tooltip("Controls the minimum vertical size that a cloud chunk will be")]
        [Range(0, 100)]
        public float minChunkHeight = 10;
        [Tooltip("Controls the maximum vertical size that a cloud chunk will be")]
        [Range(10, 1000)]
        public float maxChunkHeight = 300;
        [Range(1, 20)]
        [Tooltip("Controls how many chunks in the distance PLUME will generate. High values will lower performance")]
        public int renderDistance = 10;
        [Tooltip("Controls the height that clouds spawn at")]
        public float cloudHeight = 300;
        [Range(0, 500)]
        [Tooltip("Controls a nosie profile that changes the height that clouds spawn at")]
        public float cloudHeightDistrubution = 100;

        [Header("Noise Settings")]
        [Range(1, 10)]
        [Tooltip("Controls the size of the noise that spawns clouds")]
        public float noiseScale = 5;
        [Tooltip("Controls the seed of the noise that spawns clouds")]
        public float seed;
        [Tooltip("Controls noise scrolling speed for the cloud generation")]
        public Vector3 windSpeed = new Vector3(0.2f, 0, 0.5f);

        [Header("Normals Settings")]
        [Range(10, 5000)]
        [Tooltip("Controls the maximum distance for normal combination (combining the normals of individual clouds reduces contrast and makes the clouds seem larger")]
        public float normalizedDistance = 5000;
        
        [Tooltip("Controls height (0-1) that causes a cloud to be determined as a \"center\" for combination")]
        [Range(0, 1)]
        public float normalReferenceHeight = 0.5f;


        [Header("Color Settings")]
        [Tooltip("Multiplies the color that clouds will have in the sun")]
        [Range(0.5f, 3)]
        public float cloudColorMultiplier = 1.2f;
        [Tooltip("Multiplies the color that clouds will have in the shade")]
        [Range(0.5f, 3)]
        public float cloudShadowColorMultiplier = 1f;


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlumeProfile))]
    [CanEditMultipleObjects]
    public class E_PlumeProfile : Editor
    {

        static bool profileFolderOpen;
        PlumeProfile t;

        void OnEnable()
        {

            t = (PlumeProfile)target;

        }

        public override void OnInspectorGUI()
        {


            profileFolderOpen = EditorGUILayout.BeginFoldoutHeaderGroup(profileFolderOpen, new GUIContent($"    {t.name}"), EditorUtilities.FoldoutStyle());
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (profileFolderOpen)
            {
                EditorGUI.indentLevel++;
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudDensity"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudParticleSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("chunkSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minChunkHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxChunkHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("renderDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudHeightDistrubution"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("windSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudColorMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudShadowColorMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("normalizedDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("normalReferenceHeight"));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
        }

    }
#endif
}