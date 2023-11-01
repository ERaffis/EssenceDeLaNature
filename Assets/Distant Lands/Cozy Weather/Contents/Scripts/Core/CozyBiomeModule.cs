using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DistantLands.Cozy.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    public abstract class CozyBiomeModule : MonoBehaviour
    {

        public CozyBiome biome;
        public abstract void AddBiome();
        public abstract void RemoveBiome();

        public abstract bool CheckBiome();

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CozyBiomeModule))]
    [CanEditMultipleObjects]
    public abstract class E_BiomeModule : Editor
    {

        public abstract void DrawReports();

        public override void OnInspectorGUI()
        {
            
        }

        public abstract void DrawInlineUI(GUIStyle foldoutStyle);

    }
#endif
}