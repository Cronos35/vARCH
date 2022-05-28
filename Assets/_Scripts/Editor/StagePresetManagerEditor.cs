using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Assets._Scripts.Editor
{
    [CustomEditor(typeof(StagePresetManager))]
    public class StagePresetManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var stagePresetManager = (StagePresetManager)target;
            if (GUILayout.Button("Save stage preset"))
            {
                stagePresetManager.SaveStagePreset(); 
            }
        }
    }
}