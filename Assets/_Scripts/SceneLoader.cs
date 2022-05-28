using Assets._Scripts.Save_System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Scripts 
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private int stageScenesStartIndex;
        [SerializeField] private int stageScenesEndIndex;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private ColorManager colorManager;

        private static int currentSceneToLoadIndex = 1;
        public static int CurrentSceneToLoadindex => currentSceneToLoadIndex;
        // Start is called before the first frame update
        void Start()
        {
            //for (int i = stageScenesStartIndex; i <= stageScenesEndIndex; i++)
            //{
            //    if (SceneManager.GetSceneByBuildIndex(i).isLoaded)
            //    {
            //        currentSceneToLoadIndex = i;
            //        break;
            //    }
            //}

            #if !UNITY_EDITOR
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            #endif
        }

        public static Action<int> _onChangeStage;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
            eventSystem._onSaveDataLoaded += SetCurrentSceneIndex;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
            eventSystem._onSaveDataLoaded -= SetCurrentSceneIndex;
        }

        private void SetCurrentSceneIndex(object saveData)
        {
            SceneManager.UnloadSceneAsync(currentSceneToLoadIndex);

            vARCHData loadedSave = (vARCHData)saveData;
            currentSceneToLoadIndex = loadedSave.lastOpenedStage;

            SceneManager.LoadScene(currentSceneToLoadIndex, LoadSceneMode.Additive);
            colorManager.UpdateColorPresetOnSceneLoad(currentSceneToLoadIndex - 1);
        }

        private void OnSceneFinishedLoading(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            currentSceneToLoadIndex = loadedScene.buildIndex;
            colorManager.UpdateColorPresetOnSceneLoad(currentSceneToLoadIndex - 1);
        }

        public void LoadNextStage() 
        {
            //set last opened stage index for save system here
            if (currentSceneToLoadIndex == stageScenesEndIndex)
            {
                SceneManager.UnloadSceneAsync(currentSceneToLoadIndex);
            }

            
            currentSceneToLoadIndex = currentSceneToLoadIndex == stageScenesEndIndex ? stageScenesStartIndex : currentSceneToLoadIndex + 1;
            
            SceneManager.LoadScene(currentSceneToLoadIndex, LoadSceneMode.Additive);
            colorManager.UpdateColorPresetOnSceneLoad(currentSceneToLoadIndex - 1);

            if (currentSceneToLoadIndex > stageScenesStartIndex)
            {
                SceneManager.UnloadSceneAsync(currentSceneToLoadIndex - 1); 
            }
            _onChangeStage?.Invoke(currentSceneToLoadIndex);
        }

        public void LoadPreviousStage()
        {
            if (currentSceneToLoadIndex == stageScenesStartIndex)
            {
                SceneManager.UnloadSceneAsync(currentSceneToLoadIndex);
            }

            _onChangeStage?.Invoke(currentSceneToLoadIndex);

            currentSceneToLoadIndex = currentSceneToLoadIndex == stageScenesStartIndex ? stageScenesEndIndex : currentSceneToLoadIndex - 1;
            SceneManager.LoadScene(currentSceneToLoadIndex, LoadSceneMode.Additive);
            colorManager.UpdateColorPresetOnSceneLoad(currentSceneToLoadIndex - 1); 

            if (currentSceneToLoadIndex < stageScenesEndIndex)
            {
                SceneManager.UnloadSceneAsync(currentSceneToLoadIndex + 1);
            }
        }
    }
}