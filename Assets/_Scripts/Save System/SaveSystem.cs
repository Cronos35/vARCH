using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
//using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Assets._Scripts.Save_System
{
    /// <summary>
    /// save system will handle colors and graphics settings
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private EventSystem events;

        private vARCHData graphicsData = new vARCHData();
        private string savePath;

        private void Awake()
        {
            savePath = Application.persistentDataPath + "/stageData";
        }

        private void Start()
        {
            Load();
        }

        private void OnEnable()
        {
            events._onUpdateGraphicsSettings += SetGraphicsSettingsSaveData;
            events._onUpdateColorPresets += SetColorPresets;
#if UNITY_EDITOR

#endif
            SceneLoader._onChangeStage += SetLastStageLoaded;
        }

        private void OnDisable()
        {
            events._onUpdateGraphicsSettings -= SetGraphicsSettingsSaveData;
            events._onUpdateColorPresets -= SetColorPresets;
            SceneLoader._onChangeStage -= SetLastStageLoaded;
        }

        private void SetGraphicsSettingsSaveData(GraphicsSettingsSaveData graphicsSettingsSave)
        {
            graphicsData.graphicsSettingsSave = graphicsSettingsSave;
        }

        private void SetColorPresets(LightColors[] colorPresets)
        {
            graphicsData.colorPresets = colorPresets;
        }

        private void SetLastStageLoaded(int lastStageIndex)
        {
            graphicsData.lastOpenedStage = lastStageIndex;
        }

        public void Save()
        {
            BinaryFormatter formatter = GetBinaryFormatter();

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            FileStream file = File.Create(savePath + "/stageData.vARCH");
            formatter.Serialize(file, graphicsData);

#if UNITY_EDITOR
            Debug.Log("Save Successful");
#endif
            file.Close();
        }

        private BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            SurrogateSelector selector = new SurrogateSelector();

            ColorSerializationSurrogate colorSurrogate = new ColorSerializationSurrogate();

            selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);

            formatter.SurrogateSelector = selector;

            return formatter;
        }

        public void Load() 
        { 
            if (!File.Exists(savePath + "/stageData.vARCH"))
            {
                return;
            }

            BinaryFormatter formatter = GetBinaryFormatter();

            FileStream file = File.Open(savePath + "/stageData.vARCH", FileMode.Open);
            
            try
            {
                object save = formatter.Deserialize(file);
                file.Close();
                // no need to use invoke. Passing in parameter in the delegate calls invoke in the backend
                events._onSaveDataLoaded(save);
                graphicsData = (vARCHData)save;
#if UNITY_EDITOR
                Debug.Log("Load successful");
#endif
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("failed to load file at {0}", savePath, e);
            }
        }

        //private BinaryFormatter GetBinaryFormatter()
        //{
        //    return new BinaryFormatter;
        //}
    }

    [System.Serializable]
    public struct vARCHData
    {
        public GraphicsSettingsSaveData graphicsSettingsSave { get; set; }
        public LightColors[] colorPresets { get; set; }
        public int lastOpenedStage { get; set; }
    }
}