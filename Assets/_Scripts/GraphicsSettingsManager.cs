using System.Collections;
using UnityEngine;
using TMPro;
using Assets._Scripts.Save_System;

namespace Assets._Scripts
{
    public class GraphicsSettingsManager : MonoBehaviour
    {
        [SerializeField] private Vector2Int[] availableResolutions;
        [SerializeField] private GraphicsSettings graphicsSettings;
        [SerializeField] private EventSystem eventSystem;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI text_reflectionResolution;
        [SerializeField] private TextMeshProUGUI text_screenResolution;
        [SerializeField] private TextMeshProUGUI text_antiAliasing;
        private int resolutionsCounter = 0;

        private Vector2Int selectedResolution = new Vector2Int(1920, 1080);
        private static int reflectionQuality = 256;
        private int antiAliasingQuality = 2;
        private int reflectionResolution = 0;
        private GraphicsSettingsSaveData graphicsData = new GraphicsSettingsSaveData();

        private static GraphicsSettingsManager instance;

        public static GraphicsSettingsManager Instance
        {
            get 
            { 
                if (instance == null)
                {
                    instance = FindObjectOfType<GraphicsSettingsManager>();
                }
                return instance;
            }
        }

        public int GetReflectionSettings()
        {
            return reflectionQuality;
        }

        private void Start()
        {
            SetUIText();
            eventSystem._onUpdateGraphicsSettings(new GraphicsSettingsSaveData(resolutionsCounter, reflectionResolution, antiAliasingQuality));
        }

        private void OnEnable()
        {
            eventSystem._onSaveDataLoaded += LoadGraphicsSettingsFromSave;
        }

        private void OnDisable()
        {
            eventSystem._onSaveDataLoaded -= LoadGraphicsSettingsFromSave;
        }

        private void LoadGraphicsSettingsFromSave(object saveData)
        {
            vARCHData loadedSaveData = (vARCHData)saveData;
            reflectionResolution = loadedSaveData.graphicsSettingsSave.ReflectionResolution;
            resolutionsCounter = loadedSaveData.graphicsSettingsSave.ScreenResolution;
            antiAliasingQuality = loadedSaveData.graphicsSettingsSave.AntiAliasingQuality;

            SetUIText();
        }

        public void SetUIText()
        {
            text_screenResolution.text = availableResolutions[resolutionsCounter].x.ToString() + " x " + availableResolutions[resolutionsCounter].y.ToString();
            text_reflectionResolution.text = reflectionResolution.ToString();
            text_antiAliasing.text = antiAliasingQuality.ToString();
        }

        public void SelectResolution(bool next)
        {
            resolutionsCounter = next ? resolutionsCounter + 1 : resolutionsCounter - 1;
            if (resolutionsCounter == availableResolutions.Length || resolutionsCounter < 0)
            {
                resolutionsCounter = 0;
            }

            selectedResolution = availableResolutions[resolutionsCounter];
            SetUIText();
        }

        public void SelectReflectionResolution(bool increase) 
        {
            reflectionResolution = increase ? reflectionResolution * 2 : reflectionResolution / 2;
            if (reflectionResolution > 4096)
            {
                reflectionResolution = 4096;
            }
            else if (reflectionResolution < 64 && !increase)
            {
                reflectionResolution = 0;
            }
            else if (reflectionResolution < 64 && increase)
            {
                reflectionResolution = 64;
            }

            SetUIText();
        }

        public void SetAntiAliasingQuality(bool increase)
        {
            antiAliasingQuality = increase ? antiAliasingQuality * 2 : antiAliasingQuality / 2;
            if (antiAliasingQuality > 8)
            {
                antiAliasingQuality = 8;
            }
            else if (antiAliasingQuality < 2 && !increase)
            {
                antiAliasingQuality = 0;
            }
            else if (antiAliasingQuality == 0 && increase)
            {
                antiAliasingQuality = 2;
            }

            SetUIText();
        }

        public void ApplySettings()
        {
            QualitySettings.antiAliasing = antiAliasingQuality;
            Screen.SetResolution(availableResolutions[resolutionsCounter].x, availableResolutions[resolutionsCounter].y, false);
            eventSystem._onUpdateReflectionSettings.Invoke(reflectionResolution);

            graphicsData = new GraphicsSettingsSaveData(resolutionsCounter, reflectionResolution, antiAliasingQuality );

            eventSystem._onUpdateGraphicsSettings(graphicsData);
        }
    }

    [System.Serializable]
    public struct GraphicsSettingsSaveData
    {
        public int ScreenResolution;
        public int ReflectionResolution;
        public int AntiAliasingQuality;

        public GraphicsSettingsSaveData(int screenResolution, int reflectionResolution, int aaQuality)
        {
            ScreenResolution = screenResolution;
            ReflectionResolution = reflectionResolution;
            AntiAliasingQuality = aaQuality;
        }
    }
}