using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets._Scripts
{
    public class AudioSyncMaterial : AudioSyncer
    {
        private Material currentMaterial;
        private MeshRenderer meshRenderer;

        private Color emissionColor;
        
        // Use this for initialization

        protected override void OnStart()
        {
            base.OnStart();
            meshRenderer = GetComponent<MeshRenderer>();
            currentMaterial = meshRenderer.materials[0];
            emissionColor = currentMaterial.GetColor("_EmissionColor");
            SpectrumDataIndex = 54;
        }

        // Update is called once per frame
        private void Update()
        {
            base.OnUpdate();
            currentMaterial = meshRenderer.materials[0];

            spectrumValueHistory.Enqueue(_spectrumValue);

            if (spectrumValueHistory.Count > 10)
            {
                spectrumValueHistory.Dequeue();
            }


            //int lightMultiplier = previousSpectrumPeak > currentSpectrumPeak ? 2 : 3; 

            //previousSpectrumPeak = currentSpectrumPeak;
            //currentSpectrumPeak = spectrumValueHistory.Max();

            if (CheckSpectrumHistoryPeak())
            {
                //get the spectrum value to only oscillate between 1 and 3

                //currentMaterial.SetColor("_EmissionColor", emissionColor * spectrumValueHistory.Max());
                _ = BlinkLights();
            }
        }

        private async Task BlinkLights()
        {
            for (float i = 0; i <= _currentSpectrumPeak; i+= _currentSpectrumPeak / 4)
            {
                currentMaterial.SetColor("_EmissionColor", emissionColor * i);
                await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 100));
            }
        }
    }
}