using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Assets._Scripts
{
    //light should only flash if current spectrum data value is the highest among the last 10 values
    public class AudioSyncLight : AudioSyncer
    {
        private Light spotLight;
        

        int historyArrayIndex = 0;
        // Use this for initialization
        
        protected override void OnStart()
        {
            base.OnStart();
            SpectrumDataIndex = 0;
            spotLight = GetComponent<Light>();
        }

        // Update is called once per frame
        public override void OnUpdate()
        {
            base.OnUpdate();
            spectrumValueHistory.Enqueue(_spectrumValue);

            if (spectrumValueHistory.Count > 10)
            {
                spectrumValueHistory.Dequeue();
            }

            spectrumHistoryArray = spectrumValueHistory.ToArray();
            
            if (_spectrumValue >= spectrumValueHistory.Max())
            {
                spotLight.intensity = _spectrumValue;
            }
        }

        private float[] MoveArrayElements(float[] arrayToMove)
        {
            for (int i = 0; i < arrayToMove.Length; i++)
            {
                if (i + 2 >= arrayToMove.Length)
                {
                    break;
                }
                var currentValue = arrayToMove[i];
                var nextValue = arrayToMove[i + 1];


                arrayToMove[i + 1] = currentValue;
                arrayToMove[i + 2] = nextValue;  
            }

            return arrayToMove;
        }
    }
}