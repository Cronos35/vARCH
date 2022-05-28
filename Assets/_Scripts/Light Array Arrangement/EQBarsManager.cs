using System;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class EQBarsManager : MonoBehaviour
    {
        [SerializeField] private GameObject eqBar;
        [SerializeField] private Transform eqBarsHolder;
        [SerializeField] private Color eqBarColor = Color.yellow;
        [SerializeField] private EQBarSpectrumAssignment eQBarArrangement;
        [SerializeField] private LightArrangementShape EQBarArrangementShape;
        [SerializeField] private float _radius;
        [SerializeField] private float _spacing;
        [SerializeField] [Range(1, 360)] private float _arc;
        [SerializeField] private Transform[] eqBarTransforms;
       
        private void Start()
        {
            InitializeEQBarsOnStart();
        }

        public void InitializeEQBarsOnStart()
        {
            int spectrumDataCounter = 0;
            eqBarTransforms = new Transform[AudioSpectrum.SpectrumData.Length];
            int spectrumDataCounterReverse = AudioSpectrum.SpectrumData.Length - 1;

            //eqBarsHolder.transform.position = Vector3.zero;

            switch (eQBarArrangement)
            {
                case EQBarSpectrumAssignment.CenterPeak:
                    for (int i = 0, j = AudioSpectrum.SpectrumData.Length / 2; i < AudioSpectrum.SpectrumData.Length / 2; i++, j++)
                    {
                        //instantiate visualiser cube here and set data index to to
                        //assign spectrum data in a symmetrical manner - get the average of the current spectrum data and the next spectrum data and assign that. once index reaches 128, go in reverse and assign again. assign the spectrum value here instead

                        InstantiateEQBarsSymmetrical(spectrumDataCounter, spectrumDataCounterReverse);

                        spectrumDataCounter = spectrumDataCounter == AudioSpectrum.SpectrumData.Length - 1 ? AudioSpectrum.SpectrumData.Length : spectrumDataCounter + 2;
                        spectrumDataCounterReverse = spectrumDataCounterReverse < 0 ? 0 : spectrumDataCounterReverse - 2;


                    }
                    break;
                case EQBarSpectrumAssignment.SidePeak:
                    for (int i = 0, j = AudioSpectrum.SpectrumData.Length / 2; i < AudioSpectrum.SpectrumData.Length / 2; i++, j++)
                    {
                        InstantiateEQBarsSymmetrical(spectrumDataCounterReverse, spectrumDataCounter);

                        spectrumDataCounter = spectrumDataCounter == AudioSpectrum.SpectrumData.Length - 1 ? AudioSpectrum.SpectrumData.Length : spectrumDataCounter + 2;
                        spectrumDataCounterReverse = spectrumDataCounterReverse < 0 ? 0 : spectrumDataCounterReverse - 2;
                    }
                    break;
                case EQBarSpectrumAssignment.StaggeredPeaks:
                    break;
                default:
                    break;
            }

            eqBarTransforms = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                eqBarTransforms[i] = transform.GetChild(i).transform;
            }

            switch (EQBarArrangementShape)
            {
                case LightArrangementShape.Flat:
                    ArrangeEQBarsFlat();
                    break;
                case LightArrangementShape.Circle:
                    ArrangeEQBarsCircular();
                    break;
            }
        }

        private void ArrangeEQBarsFlat()
        {
            for (int i = 0, j = eqBarTransforms.Length - 1; i < eqBarTransforms.Length / 2; i++, j--)
            {
                eqBarTransforms[i].localPosition = new Vector3(0, 0, (i - eqBarTransforms.Length / 2) * _spacing);
                eqBarTransforms[j].localPosition = new Vector3(0, 0, (j - eqBarTransforms.Length / 2) * _spacing);
            }
        }

        private void InstantiateEQBarsSymmetrical(int spectrumDataCounter, int spectrumDataCounterReverse)
        {
            AudioSyncScale visualiserCubeScaler = Instantiate(eqBar).GetComponent<AudioSyncScale>();
            AudioSyncScale visualiserCubeScalerReverse = Instantiate(eqBar).GetComponent<AudioSyncScale>();

            visualiserCubeScaler.spectrumRangeAssignment = SpectrumRangeAssignment.FullRange;
            visualiserCubeScalerReverse.spectrumRangeAssignment = SpectrumRangeAssignment.FullRange;

            visualiserCubeScaler.SetGlowColor(eqBarColor);
            visualiserCubeScalerReverse.SetGlowColor(eqBarColor);

            visualiserCubeScalerReverse.spectrumRangeAssignment =
                visualiserCubeScaler.spectrumRangeAssignment = SpectrumRangeAssignment.FullRange;

            visualiserCubeScaler.SpectrumDataIndex = spectrumDataCounterReverse;
            visualiserCubeScalerReverse.SpectrumDataIndex = spectrumDataCounter;

            visualiserCubeScaler.transform.SetParent(eqBarsHolder.transform);
            visualiserCubeScalerReverse.transform.SetParent(eqBarsHolder.transform);
        }
        
        private void ArrangeEQBarsCircular()
        {
            if(EQBarArrangementShape != LightArrangementShape.Circle)
            {
                return;
            }

            for (int i = 0; i < eqBarTransforms.Length; i++)
            {
                Transform currentEqBar = eqBarTransforms[i]; 
                GameObject eqBarParent = new GameObject("eqBarHolder");
                Vector3 eqBarPosition = new Vector3();

                eqBarParent.transform.SetParent(transform);
                currentEqBar.SetParent(eqBarParent.transform);

                eqBarParent.transform.localScale = Vector3.one;
                eqBarParent.transform.localPosition = new Vector3();
                
                eqBarPosition.x = _radius;
                currentEqBar.localPosition = eqBarPosition;
                eqBarParent.transform.localRotation = Quaternion.Euler(new Vector3(0, -90 + (_arc / eqBarTransforms.Length * i), 0));
            }
        }
    }
}