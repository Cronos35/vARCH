using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class LightStickCrowdArranger : MonoBehaviour
    {
        [SerializeField] private GameObject lightStickPrefab;
        [SerializeField] private EventSystem eventSystem;
        [Header("Light Stick Transforms")]
        [SerializeField] private float foreAftPositionMin = 1;
        [SerializeField] private float foreAftPositionMax = 5;
        [SerializeField] private float sidePositionMin = 1;
        [SerializeField] private float sidePositionMax = 10;
        [SerializeField] private float crowdRowSpacing = 1.5f;
        [SerializeField] private float crowdColumnSpacing = 1.5f;
        [Header("Sway Settings")]
        [SerializeField] private float swayTimeMin = 0.2f;
        [SerializeField] private float swayTimeMax = 2f;
        [SerializeField] private float swayLimitMin = 5f;
        [SerializeField] private float swayLimitMax = 70f;
        [Header("Light stick counts and colors")]
        [SerializeField] private int lightStickCount;
        [SerializeField] private float lightStickIntensity;
        [SerializeField] private Color[] lightStickColors;
        private Transform[] crowdTransforms;
        private LightStickSway[] lightSticks;

        private void OnEnable()
        {
            eventSystem._onUpdateStageColors += ReceiveColors;
        }

        private void OnDisable()
        {
            eventSystem._onUpdateStageColors -= ReceiveColors;
        }

        private void Awake()
        {
            //lightStickColors = new Color[3];
            lightSticks = new LightStickSway[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                lightSticks[i] = transform.GetChild(i).GetComponent<LightStickSway>();
            }
            SetLightStickColor();
        }

        private void ReceiveColors(LightColors lightColors)
        {
            lightStickColors[0] = lightColors.primaryColor;
            lightStickColors[1] = lightColors.secondaryColor;
            lightStickColors[2] = lightColors.tertiaryColor;

            SetLightStickColor();
        }

        public void InitializeCrowds()
        {
            crowdTransforms = new Transform[lightStickCount];
            lightSticks = new LightStickSway[lightStickCount];
            RemoveAll();

            for (int i = 0; i < lightStickCount; i++)
            {
                Transform currentLightStick = Instantiate(lightStickPrefab).transform;
                crowdTransforms[i] = currentLightStick;
                currentLightStick.SetParent(transform);
                currentLightStick.localPosition = new Vector3(Random.Range(foreAftPositionMin, foreAftPositionMax), 0, Random.Range(sidePositionMin, sidePositionMax));
                lightSticks[i] = currentLightStick.gameObject.GetComponent<LightStickSway>();
            }

            for (int i = 0; i < lightSticks.Length; i++)
            {
                lightSticks[i].swayTime = Random.Range(swayTimeMin, swayTimeMax);
                lightSticks[i].swayLimit = Random.Range(swayLimitMin, swayLimitMax);
            }

            SetLightStickColor();
        }

        public void SetLightStickColor()
        {
            if (lightSticks == null || lightSticks[0] == null)
            {
                return;
            }

            for (int i = 0; i < lightSticks.Length; i++)
            {
                lightSticks[i].glowColor = lightStickColors[Random.Range(0, lightStickColors.Length)];
                lightSticks[i].lightIntensity = lightStickIntensity;
            }
        }


        public void RemoveAll()
        {
            if (transform.childCount <= 0)
            {
                return;
            }

            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}