using Assets._Scripts.Save_System;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets._Scripts
{
    public class ColorManager : MonoBehaviour
    {
        [SerializeField] private FlexibleColorPicker primaryColorPicker;
        [SerializeField] private FlexibleColorPicker secondaryColorPicker;
        [SerializeField] private FlexibleColorPicker tertiaryColorPicker;
        //[SerializeField] private Transform crowdColorsPanel;
        //[SerializeField] private GameObject colorPickePanel;
        [SerializeField] private EventSystem eventSystem;
        //[SerializeField] private int lightStickColorCount = 4;
        [SerializeField] private LightColors[] colorPresets;

        private LightColors lightColors;
        private LightColors previousColors;
        private int currentSceneIndex = 0; //  this is used to load the scene by build index. stage build indices start at 1
        
        private void Start()
        {
            UpdateStageColors();
            eventSystem._onUpdateColorPresets(colorPresets);
        }

        private void UpdateStageColors()
        {
            lightColors = colorPresets[SceneLoader.CurrentSceneToLoadindex - 1];
            primaryColorPicker.color = lightColors.primaryColor;
            secondaryColorPicker.color = lightColors.secondaryColor;
            tertiaryColorPicker.color = lightColors.tertiaryColor;

            eventSystem._onUpdateStageColors(lightColors);
        }

        private void OnEnable()
        {
            primaryColorPicker.onColorChange.AddListener((color) => {
                lightColors.primaryColor = color;
                colorPresets[currentSceneIndex].primaryColor = color;
                eventSystem._onUpdateStageColors.Invoke(lightColors);
                eventSystem._onUpdateColorPresets(colorPresets);
                //currentColorSet.primaryColor = color;
            });

            secondaryColorPicker.onColorChange.AddListener((color) => {
                lightColors.secondaryColor = color;
                colorPresets[currentSceneIndex].secondaryColor = color;
                eventSystem._onUpdateColorPresets(colorPresets);
                eventSystem._onUpdateStageColors.Invoke(lightColors);
                //currentColorSet.secondaryColor = color;
            });

            tertiaryColorPicker.onColorChange.AddListener((color) => {
                lightColors.tertiaryColor = color;
                colorPresets[currentSceneIndex].tertiaryColor = color;
                eventSystem._onUpdateColorPresets(colorPresets);
                eventSystem._onUpdateStageColors.Invoke(lightColors);
                //currentColorSet.tertiaryColor = color;
            });

            eventSystem._onSaveDataLoaded += LoadColorsFromSave;
        }

        //this will only take effect at the beginning of the game
        private void LoadColorsFromSave(object save)
        {
            vARCHData loadedSave = (vARCHData)save;
            colorPresets = loadedSave.colorPresets;
            eventSystem._onUpdateStageColors(colorPresets[loadedSave.lastOpenedStage - 1]);
            eventSystem._onUpdateColorPresets(colorPresets);
            UpdateStageColors();
        }


        private void OnDisable()
        {
            primaryColorPicker.onColorChange.RemoveAllListeners();
            secondaryColorPicker.onColorChange.RemoveAllListeners();
            tertiaryColorPicker.onColorChange.RemoveAllListeners();
            eventSystem._onSaveDataLoaded -= LoadColorsFromSave;
        }

        public void UpdateColorPresetOnSceneLoad(int colorSetIndex)
        {
            if (eventSystem._onUpdateStageColors == null)
            {
                return;
            }
            currentSceneIndex = colorSetIndex;

            //if (sceneIndex > 1)
            //{
            //    colorPresets[sceneIndex - 1] = new LightColors(lightColors.primaryColor, lightColors.secondaryColor, lightColors.tertiaryColor, ColorAssignmentMode.Alternating);
            //}
            //if (colorSetIndex > 0)
            //{
            //    colorPresets[colorSetIndex - 1] = new LightColors(lightColors.primaryColor, lightColors.secondaryColor, lightColors.tertiaryColor, ColorAssignmentMode.Alternating);
            //}
            
            //previousColors = new LightColors(lightColors.primaryColor, lightColors.secondaryColor, lightColors.tertiaryColor, ColorAssignmentMode.Alternating);
            lightColors = colorPresets[colorSetIndex];

            primaryColorPicker.color = colorPresets[colorSetIndex].primaryColor;
            secondaryColorPicker.color = colorPresets[colorSetIndex].secondaryColor;
            tertiaryColorPicker.color = colorPresets[colorSetIndex].tertiaryColor;
            
            eventSystem._onUpdateStageColors.Invoke(colorPresets[colorSetIndex]);
        }
    }
}