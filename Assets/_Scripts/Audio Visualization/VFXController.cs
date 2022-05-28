using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets._Scripts
{
    public class VFXController : AudioSyncer
    {
        private VisualEffect vfx;
        [SerializeField] private int spectrumIndex;

        // Use this for initialization
        
        protected override void OnStart()
        {
            vfx = GetComponent<VisualEffect>();
            SpectrumDataIndex = spectrumIndex;
            base.OnStart();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (CheckSpectrumHistoryPeak())
            {
                vfx.Play();
            }

            vfx.SetFloat("Size", _spectrumValue / 5);
            vfx.SetFloat("PerlinNoiseValue", _spectrumValue / 10);
            //vfx.SetFloat("VFXIntensity", _spectrumValue * 100);

            //if (vfx.GetFloat("VFXIntensity") > 0)
            //{

            //}
        }
    }
}