using System.Collections;
using Assets._Scripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EQBarsManager))]
public class EQBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var visualizationManager = (EQBarsManager)target;

        if (GUILayout.Button("Create EQ bars"))
        {
            visualizationManager.InitializeEQBarsOnStart();
        }
    }
}