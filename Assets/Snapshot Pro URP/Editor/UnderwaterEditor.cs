namespace SnapshotShaders.URP
{
    using UnityEditor.Rendering;
    using UnityEngine.Rendering.Universal;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

#if UNITY_2022_2_OR_NEWER
    [CustomEditor(typeof(UnderwaterSettings))]
#else
    [VolumeComponentEditor(typeof(UnderwaterSettings))]
#endif
    public class UnderwaterEditor : VolumeComponentEditor
    {
        SerializedDataParameter bumpMap;
        SerializedDataParameter strength;
        SerializedDataParameter waterFogColor;
        SerializedDataParameter fogStrength;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<UnderwaterSettings>(serializedObject);
            bumpMap = Unpack(o.Find(x => x.bumpMap));
            strength = Unpack(o.Find(x => x.strength));
            waterFogColor = Unpack(o.Find(x => x.waterFogColor));
            fogStrength = Unpack(o.Find(x => x.fogStrength));
        }

        public override void OnInspectorGUI()
        {
            if (!SnapshotUtility.CheckEffectEnabled<Underwater>())
            {
                EditorGUILayout.HelpBox("The Underwater effect must be added to your renderer's Renderer Features list.", MessageType.Error);
                if (GUILayout.Button("Add Underwater Renderer Feature"))
                {
                    SnapshotUtility.AddEffectToPipelineAsset<Underwater>();
                }
            }

            PropertyField(bumpMap);
            PropertyField(strength);
            PropertyField(waterFogColor);
            PropertyField(fogStrength);
        }

#if UNITY_2021_2_OR_NEWER
        public override GUIContent GetDisplayTitle()
        {
            return new GUIContent("Underwater");
        }
#else
    public override string GetDisplayTitle()
    {
        return "Underwater";
    }
#endif
    }
}
