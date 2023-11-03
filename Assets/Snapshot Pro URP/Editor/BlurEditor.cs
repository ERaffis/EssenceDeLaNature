﻿namespace SnapshotShaders.URP
{
    using UnityEngine;
    using UnityEditor.Rendering;
    using UnityEngine.Rendering.Universal;
    using System.Reflection;
    using UnityEditor;

#if UNITY_2022_2_OR_NEWER
    [CustomEditor(typeof(BlurSettings))]
#else
    [VolumeComponentEditor(typeof(BlurSettings))]
#endif
    public class BlurEditor : VolumeComponentEditor
    {
        SerializedDataParameter strength;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<BlurSettings>(serializedObject);
            strength = Unpack(o.Find(x => x.strength));
        }

        public override void OnInspectorGUI()
        {
            if (!SnapshotUtility.CheckEffectEnabled<Blur>())
            {
                EditorGUILayout.HelpBox("The Blur effect must be added to your renderer's Renderer Features list.", MessageType.Error);
                if (GUILayout.Button("Add Blur Renderer Feature"))
                {
                    SnapshotUtility.AddEffectToPipelineAsset<Blur>();
                }
            }

            PropertyField(strength);
        }

#if UNITY_2021_2_OR_NEWER
        public override GUIContent GetDisplayTitle()
        {
            return new GUIContent("Blur");
        }
#else
    public override string GetDisplayTitle()
    {
        return "Blur";
    }
#endif
    }
}
