using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets._Scripts
{
    public class LightArrayController : AudioSyncer
    {
        [SerializeField] private Color lightColor;
        [SerializeField] private int spectrumIndex;
        [SerializeField] private ArrayControllerBehavior controllerBehavior;
        //public float lightIntensityMultiplier;

        private LightRowController[] lightRowControllers;
        private int currentLightRow = 0;

        public TriggerSequence triggerSequence;
        public Color LightColor
        {
            get
            {
                return lightColor;
            }
            set
            {
                lightColor = value;
                if (lightRowControllers == null || lightRowControllers.Length <= 0)
                {
                    return;
                }
                for (int i = 0; i < lightRowControllers.Length; i++)
                {
                    lightRowControllers[i].SetLightColorEditor(value);
                }
            }
        }


        protected override void OnStart()
        {
            base.OnStart();
            GetLightarrayControllers();
        }


        public void GetLightarrayControllers()
        {
            SpectrumDataIndex = spectrumIndex;

            lightRowControllers = new LightRowController[transform.childCount];
            for (int i = 0; i < lightRowControllers.Length; i++)
            {
                lightRowControllers[i] = transform.GetChild(i).GetComponent<LightRowController>();
            }
        }

        public void InitializeLightArrayControllerEditor()
        {
            lightRowControllers = new LightRowController[transform.childCount];
            for (int i = 0; i < lightRowControllers.Length; i++)
            {
                lightRowControllers[i] = transform.GetChild(i).GetComponent<LightRowController>();
                lightRowControllers[i].InitializeLights();
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //when CheckPeakSpectrumValue() returns true, turn off previous light row and turn on currentlightrow

            if (!CheckSpectrumHistoryPeak())
            {
                return;
            }
            //if (_idle)
            //{
            //    return;
            //}

            if (controllerBehavior == ArrayControllerBehavior.Sequential)
            {
                UniTask sequenceLights = TriggerLightsSequential();
                return;
            }

            lightRowControllers[currentLightRow].ToggleLights(_currentSpectrumPeak, lightColor);
            currentLightRow = currentLightRow + 1 == lightRowControllers.Length ? 0 : currentLightRow + 1;

            if (currentLightRow > 0 && currentLightRow < transform.childCount - 1)
            {
                lightRowControllers[currentLightRow - 1].ToggleLights(0, lightColor);
                //lightRowControllers[currentLightRow + 1].ToggleLights(0, lightColor, _timeBetweenPeaks);
            }
                
            _timeBetweenPeaks = 0;

            //the total time for this task to finish should be TimebetweenPeaks * 2
        }
        private async UniTask TriggerLightsSequential()
        {
            if (triggerSequence == TriggerSequence.Forward) 
            {
                for (int i = 0; i < lightRowControllers.Length; i++)
                {
                    lightRowControllers[i].ToggleLights(_currentSpectrumPeak, lightColor);
                    if (i > 0)
                    {
                        lightRowControllers[i - 1].ToggleLights(0, lightColor);
                    }
                    await UniTask.Delay(Mathf.RoundToInt(((_timeBetweenPeaks * 100) * 2 ) / lightRowControllers.Length));
                }
            }
            
            else if(triggerSequence == TriggerSequence.Reverse)
            {
                for (int i = lightRowControllers.Length - 1; i >= 0; i--)
                {
                    lightRowControllers[i].ToggleLights(_currentSpectrumPeak, lightColor);
                    if (i < lightRowControllers.Length && i > 0)
                    {
                        lightRowControllers[i - 1].ToggleLights(0, lightColor);
                    }
                    await UniTask.Delay(Mathf.RoundToInt(((_timeBetweenPeaks * 100) * 2) / lightRowControllers.Length));
                }
            }
        }




        [System.Serializable]
        private enum ArrayControllerBehavior 
        { 
            Single,
            Sequential
        }

        public enum TriggerSequence
        {
            Forward,
            Reverse
        }
    }
}