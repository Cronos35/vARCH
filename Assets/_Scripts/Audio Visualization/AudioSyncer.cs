using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.WasapiAudio;
namespace Assets._Scripts
{
    //all audio analysis is being done here 
    public class AudioSyncer : MonoBehaviour
    {
        public SpectrumRangeAssignment spectrumRangeAssignment;
        protected Queue<float> spectrumValueHistory = new Queue<float>();
        protected float[] spectrumHistoryArray = new float[20];
        protected float[] spectrumRange;
        //protected float[] 

        
        protected float _spectrumValue;
        protected float _currentSpectrumPeak;
        protected float _timeBetweenPeaks;
        protected float previousSpectrumPeak;
        public int SpectrumDataIndex { get; set; } // the index in the spectrum data where audio syncer gets the 
        private int rangeWidth;
        private int rangeBottom = 0;
        private int rangeTop = 0;

        private void Start()
        {
            //assign spectrum range based on 
            OnStart();
        }

        protected virtual void OnStart()
        {
            rangeWidth = AudioSpectrum.SpectrumData.Length / 5;
            spectrumRange = new float[rangeWidth];
        }

        private void Update()
        {
            OnUpdate();
        }

        public virtual void OnUpdate()
        {
            _timeBetweenPeaks += Time.deltaTime;

            if (AudioSpectrum.SpectrumData == null)
            {
                return;
            }

            if (AudioSpectrum.SpectrumData.Length == 0)
            {
                return;
            }

            if (SpectrumDataIndex + 1 == AudioSpectrum.SpectrumData.Length)
            {
                _spectrumValue = AudioSpectrum.SpectrumData[SpectrumDataIndex];
                return;
            }
                        
            spectrumValueHistory.Enqueue(_spectrumValue);
            _spectrumValue = AudioSpectrum.SpectrumData[SpectrumDataIndex] * 100;

            if (spectrumValueHistory.Count > 10)
            {
                spectrumValueHistory.Dequeue();
            }

            _currentSpectrumPeak = spectrumValueHistory.Max() * 100;

            if (spectrumRangeAssignment == SpectrumRangeAssignment.FullRange)
            {
                return;
            }

            if(spectrumRange.Length <= 0)
            {
                return;
            }

            if (AudioSpectrum.SpectrumData.Average() < 2)
            {
                //_idle = true;
                return;
            }
            //_idle = false;

            switch (spectrumRangeAssignment)
            {
                case SpectrumRangeAssignment.DeepBass:
                    rangeTop = AudioSpectrum.SpectrumData.Length - 1;
                    break;
                case SpectrumRangeAssignment.Bass:
                    rangeTop = rangeWidth * 4;
                    break;
                case SpectrumRangeAssignment.MidRange:
                    rangeTop = rangeWidth * 3;
                    break;
                case SpectrumRangeAssignment.UpperRange:
                    rangeTop = rangeWidth * 2;
                    break;
                case SpectrumRangeAssignment.TopRange:
                    rangeTop = rangeWidth;
                    break;
            }

            rangeBottom = rangeTop - rangeWidth;

            for (int i = 0, j = rangeBottom; i < rangeWidth; i++, j++)
            {
                spectrumRange[i] = AudioSpectrum.SpectrumData[j];
            }
        }


        protected bool CheckSpectrumHistoryPeak()
        {
            previousSpectrumPeak = _currentSpectrumPeak;
            _currentSpectrumPeak = spectrumValueHistory.Max();

            if (previousSpectrumPeak < _currentSpectrumPeak)
            {
                _timeBetweenPeaks = 0;
            }

            if (_spectrumValue >= spectrumValueHistory.Max())
            {
                //get the spectrum value to only oscillate between 1 and 3
                return true;
            }
            return false;
        }
    }

    public enum SpectrumRangeAssignment
    {
        TopRange,
        UpperRange,
        MidRange,
        Bass,
        DeepBass,
        FullRange
    }
}