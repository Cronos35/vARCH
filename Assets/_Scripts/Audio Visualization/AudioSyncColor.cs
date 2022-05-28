using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets._Scripts
{
    public class AudioSyncColor : AudioSyncer
    {
        [SerializeField] private int glowMaterialIndex;
        [SerializeField] private Gradient _colorGradient;
        [SerializeField] private float colorTransitionTime = 2;
        private float time = 0;
        private Material glowMaterial;

        protected override void OnStart()
        {
            base.OnStart();
            glowMaterial = GetComponent<MeshRenderer>().materials[glowMaterialIndex];
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            float gradientValue = Mathf.Lerp(0f, 1f, time);

            if (CheckSpectrumHistoryPeak())
            {
                time = time > 1 ? 0 : time + (Time.deltaTime /2);
            }

            float spectrumMax = _currentSpectrumPeak;
            float spectrumMin = spectrumValueHistory.Min();
            Color color = _colorGradient.Evaluate(gradientValue);
            glowMaterial.SetColor("_EmissionColor", color * 2);
        }
    }
}