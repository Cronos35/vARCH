using UnityEngine.Rendering;
using UnityEngine;

namespace Assets._Scripts
{
    public class ReflectionsSetter : MonoBehaviour
    {
        [SerializeField] private EventSystem gameEvents;
        private ReflectionProbe reflectionProbe;

        private void Awake()
        {
            reflectionProbe = GetComponent<ReflectionProbe>();
        }

        private void Start()
        {
            SetReflectionSettings(GraphicsSettingsManager.Instance.GetReflectionSettings()); 
        }

        private void OnEnable()
        {
            gameEvents._onUpdateReflectionSettings += SetReflectionSettings;
        }

        private void OnDisable()
        {
            gameEvents._onUpdateReflectionSettings -= SetReflectionSettings;
        }

        private void SetReflectionSettings(int reflectionQuality)
        {
            if (reflectionQuality == 0)
            {
                //bake reflection probe here;
                reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
                return; 
            }
            reflectionProbe.resolution = reflectionQuality;
            reflectionProbe.refreshMode = ReflectionProbeRefreshMode.EveryFrame;
        }
    }
}