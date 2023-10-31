using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EmptyShaderGUI : MaterialEditor
{

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Material generated at runtime by COZY: Stylized Weather 2. Edit material properties within COZY's editor", MessageType.Info);
    }
}
