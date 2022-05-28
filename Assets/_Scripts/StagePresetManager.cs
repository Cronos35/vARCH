using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class StagePresetManager : MonoBehaviour
    {
        [SerializeField] private StagePreset stagePreset;
        [SerializeField] private EventSystem eventSystem;

        public void SaveStagePreset()
        {
            eventSystem._onSaveStageData(stagePreset);
        }

        public void LoadStagePreset()
        {

        }
    }
}