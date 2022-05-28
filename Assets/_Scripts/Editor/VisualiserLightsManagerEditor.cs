using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Assets._Scripts.Editor
{
    [CustomEditor(typeof(VisualiserLightsManager)), CanEditMultipleObjects]
    public class VisualiserLightsManagerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VisualiserLightsManager lightBlockManager = (VisualiserLightsManager)target;

            if(GUILayout.Button("Initialize light blocks"))
            {
                lightBlockManager.InitializeLightBlocks();
            }

            if (GUILayout.Button("Set colors"))
            {
                lightBlockManager.SetColors();
            }

            if (GUILayout.Button("Remove All"))
            {
                lightBlockManager.RemoveAll();
            }

            if (GUILayout.Button("Save stage preset"))
            {
                lightBlockManager.SaveLightBlockData(lightBlockManager._stagePreset); 
            }

            if(GUILayout.Button("Load stage preset"))
            {
                lightBlockManager.LoadLightBlockPreset();
            }

            if (GUILayout.Button("Get visualiser block components"))
            {
                lightBlockManager.VisualiserLightBlocks = new VisualiserLightBlock[lightBlockManager.transform.childCount];
                for (int i = 0; i < lightBlockManager.transform.childCount; i++)
                {
                    lightBlockManager.VisualiserLightBlocks[i] = lightBlockManager.transform.GetChild(i).GetComponent<VisualiserLightBlock>();
                }
            }

            if (lightBlockManager.overrideBlockSettings)
            {
                lightBlockManager.SetLightBlockTransforms();
                lightBlockManager.SetLightArrangementSettings();
            }
        }
    }
}