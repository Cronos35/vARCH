using Assets._Scripts;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(VisualiserLightBlock)), CanEditMultipleObjects]
public class VisualiserLightBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var visualiserLightBlock = (VisualiserLightBlock)target;
        //have transforms of each of the lights in the light array be changed here

        visualiserLightBlock.lightObject = visualiserLightBlock._lightPrefabs[(int)visualiserLightBlock._selectedLightType];
        visualiserLightBlock.ArrangeLights();
        visualiserLightBlock.SetArrayRotationSpeed();
        visualiserLightBlock.SetArraySpinDirection();
        visualiserLightBlock.UpdateLightBlockProperties();
        
        if (!EditorApplication.isPlaying) 
        { 
            visualiserLightBlock.SetColors();
            visualiserLightBlock.SetLightArrayColors();
        }

        if (GUILayout.Button("Update light block properties"))
        {
            visualiserLightBlock.UpdateLightBlockProperties();
        }

        if (GUILayout.Button("Initialize Lights"))
        {

            visualiserLightBlock.InitializeLightArray();
            Color[] currentColors = visualiserLightBlock._lightColors;
            visualiserLightBlock._lightColors = new Color[visualiserLightBlock.transform.childCount];

            for (int i = 0, j = 0; i < visualiserLightBlock._lightColors.Length; i++)
            {
                j = j + 1 == currentColors.Length ? 0 : j + 1;
                visualiserLightBlock._lightColors[i] = currentColors[j];
            }
        }

        if (GUILayout.Button("Remove all lights"))
        {
            visualiserLightBlock.RemoveAll();
        }

        if(GUILayout.Button("Get light array transforms"))
        {
            visualiserLightBlock.ArrayTransforms = new Transform[visualiserLightBlock._lightArrayControllerCount];
            visualiserLightBlock.LightRowTransforms = new Transform[visualiserLightBlock._lightRowsPerController * visualiserLightBlock._lightArrayControllerCount] ;
            visualiserLightBlock.LightTransforms = new Transform[visualiserLightBlock._lightsPerRow * visualiserLightBlock._lightRowsPerController * visualiserLightBlock._lightArrayControllerCount];

            for (int arrayCounter = 0, rowCounter = 0, lightCounter = 0; arrayCounter < visualiserLightBlock.transform.childCount; arrayCounter++)
            {
                Transform currentArray = 
                    visualiserLightBlock.ArrayTransforms[arrayCounter] = visualiserLightBlock.transform.GetChild(arrayCounter).transform;
                for (int i = 0; i < currentArray.transform.childCount; i++)
                {
                    Transform currentRow = currentArray.GetChild(i).transform;
                    visualiserLightBlock.LightRowTransforms[rowCounter] = currentRow;
                    rowCounter++;

                    for (int j = 0; j < currentRow.transform.childCount; j++)
                    {
                        visualiserLightBlock.LightTransforms[lightCounter] = currentRow.transform.GetChild(j).transform;
                        lightCounter++;
                    }
                }
            }
        }
    }
}
