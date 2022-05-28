using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    //stage preset scriptable object will store all the data for the visualiser light blocks. Visualiser light blocks will also use this scriptable object to write their data onto the scriptable object
    [CreateAssetMenu(menuName = "Stage Preset", fileName ="New Stage Preset")]
    public class StagePreset : ScriptableObject
    {
        public LightBlockProperties[] lightBlockProperties;
    }
}