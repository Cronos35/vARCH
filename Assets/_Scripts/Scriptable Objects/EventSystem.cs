using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    [CreateAssetMenu(fileName ="New Event System", menuName = "Event System")]
    public class EventSystem : ScriptableObject
    {
        public delegate void EmitCamera(Camera camera);
        public delegate void SaveStageData(StagePreset stagePreset);
        public delegate void UpdateStageColors(LightColors lightColors);
        public delegate void UpdateColorPresets(LightColors[] colorPresets);
        public delegate void UpdateCrowdColors(Color[] crowdColors);
        public delegate void UpdateReflectionsSettings(int reflectionQuality);
        public delegate void UpdateGraphicsSettings(GraphicsSettingsSaveData graphicsSettingsSaveData);
        public delegate void SaveDataLoaded(object saveData);

        public SaveDataLoaded _onSaveDataLoaded;
        public UpdateColorPresets _onUpdateColorPresets;
        public UpdateGraphicsSettings _onUpdateGraphicsSettings;
        public UpdateReflectionsSettings _onUpdateReflectionSettings;
        public UpdateCrowdColors _onUpdateCrowdColors;
        public UpdateStageColors _onUpdateStageColors;
        public SaveStageData _onSaveStageData;
        public EmitCamera _onEmitCamera;
    }
}