using UnityEditor;
using UnityEngine;
using Assets._Scripts.Save_System;

[CustomEditor(typeof(SaveSystem))]
public class SaveSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SaveSystem saveSystem = (SaveSystem)target;

        if (GUILayout.Button("Save"))
        {
            //saveSystem.Save();
        }
    }
}
