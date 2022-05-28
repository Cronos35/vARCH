using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Assets._Scripts.Editor
{
    [CustomEditor(typeof(LightStickCrowdArranger))]
    public class LightStickCrowdEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var lightStickCrowdArranger = (LightStickCrowdArranger)target;
            base.OnInspectorGUI();

            if (GUILayout.Button("Initialize crowds"))
            {
                lightStickCrowdArranger.InitializeCrowds();
            }

            if (GUILayout.Button("Set light stick colors")) 
            {
                lightStickCrowdArranger.SetLightStickColor();
            }
        }
    }
}