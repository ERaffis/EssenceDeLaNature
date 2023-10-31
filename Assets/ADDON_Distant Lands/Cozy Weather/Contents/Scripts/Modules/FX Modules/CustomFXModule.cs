using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    [Serializable]
    public class CustomFXModule : CozyFXModule
    {

        public override void OnFXEnable()
        {
            
            if (parent == null)
                SetupFXParent();
            else if (parent.parent == null)
                parent.parent = vfx.parent;
                
            //Use this method to run commands when your FX module is enabled. Also runs on Start.
        }

        public override void OnFXUpdate()
        {


            if (!isEnabled)
                return;

            if (vfx == null)
                vfx = CozyWeather.instance.GetModule<VFXModule>();

            if (parent == null)
                SetupFXParent();
            else if (parent.parent == null)
                parent.parent = vfx.parent;

            parent.transform.localPosition = Vector3.zero;

            //Use this method like an update loop for your FX module. All the code already here just sets up the module if things are not properly setup yet.

        }

        public override void OnFXDisable()
        {
                
            if (parent)
                MonoBehaviour.DestroyImmediate(parent.gameObject);

            //Use this method to run commands when your FX module is disabled. Also runs when a scene is closed, FX are disabled, etc.
            
        }

        public override void SetupFXParent()
        {
            
            if (vfx.parent == null)
                return;

            parent = new GameObject().transform;
            parent.parent = vfx.parent;
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;
            parent.name = "Custom FX";
            parent.gameObject.AddComponent<FXParent>();
            
        }

    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(CustomFXModule))]
    public class CustomFXModuleDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            Rect pos = position;

            Rect tabPos = new Rect(pos.x + 35, pos.y, pos.width - 41, pos.height);
            Rect togglePos = new Rect(5, pos.y, 30, pos.height);

            property.FindPropertyRelative("_OpenTab").boolValue = EditorGUI.BeginFoldoutHeaderGroup(tabPos, property.FindPropertyRelative("_OpenTab").boolValue, new GUIContent("    Custom FX", "This is your custom FX module."), EditorUtilities.FoldoutStyle());

            bool toggle = EditorGUI.Toggle(togglePos, GUIContent.none, property.FindPropertyRelative("_IsEnabled").boolValue);

            if (property.FindPropertyRelative("_IsEnabled").boolValue != toggle)
            {
                property.FindPropertyRelative("_IsEnabled").boolValue = toggle;

                if (toggle == true)
                    (property.serializedObject.targetObject as VFXModule).audioManager.OnFXEnable();
                else
                    (property.serializedObject.targetObject as VFXModule).audioManager.OnFXDisable();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            if (property.FindPropertyRelative("_OpenTab").boolValue)
            {
                using (new EditorGUI.DisabledScope(!property.FindPropertyRelative("_IsEnabled").boolValue))
                {

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    //____________________________________________________Add your FX module variables here:
                    // EditorGUILayout.PropertyField(property.FindPropertyRelative("myCustomProperty1"));
                    // EditorGUILayout.PropertyField(property.FindPropertyRelative("myCustomProperty2"));

                    EditorGUI.indentLevel--;

                }

            }


            EditorGUI.EndProperty();
        }

    }
#endif
}
